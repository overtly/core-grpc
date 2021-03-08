using Com.Ctrip.Framework.Apollo;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Overt.Core.Grpc;
using Overt.GrpcExample.Client.Tracer;
using System;
using System.Threading;

namespace Overt.GrpcExample.Client
{
    class Program
    {
        static IServiceProvider provider;
        static Program()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", true);

            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddOptions();

            // 注入GrpcClient
            services.AddGrpcClient<ConsoleTracer>();
            services.Configure<GrpcClientOptions>(options =>
            {
                options.Interceptors.Add(new ClientLoggerInterceptor());
            });
            // 第三方配置，启动可用
            //services.AddGrpcConfig(config =>
            //{
            //    config.AddApollo(configuration.GetSection("apollo")).AddDefault();
            //});
            //services.Configure<GrpcClientOptions<GrpcExampleServiceClient>>(cfg =>
            //{
            //    cfg.ConfigPath = "";
            //});

            provider = services.BuildServiceProvider();
        }

        static void Main(string[] args)
        {
            do
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.A)
                    break;
                var service = provider.GetService<IGrpcClient<Service.Grpc.GrpcExampleService.GrpcExampleServiceClient>>();

                try
                {
                    service.Client(1000).Ask(new Service.Grpc.AskRequest() { Key = "abc" });

                    //service.CreateClient((serverCallInvokers) =>
                    //{
                    //    //TODO 自定义策略，可以根据UserId 进行路由节点等
                    //    return serverCallInvokers[0];

                    //}).Ask(new Service.Grpc.AskRequest() { Key = "abc" });

                    //var res = client.Client.Ask(new Service.Grpc.AskRequest() { Key = "abc" });
                    //Console.WriteLine(DateTime.Now + " - " + res.Content ?? "abc");
                }
                catch (Exception ex)
                {
                    var res = service.Client.Ask(new Service.Grpc.AskRequest() { Key = "abc" });

                    Console.WriteLine($"{DateTime.Now} - 异常");
                }

                Thread.Sleep(200);

            } while (true);

            Console.WriteLine("over");
            Console.ReadLine();
        }
    }

    public static class GrpcClientExtensions
    {
        /// <summary>
        /// 扩展一
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static T Client<T>(this IGrpcClient<T> data, long userId) where T : ClientBase
        {
            return data.CreateClient((servercallInvokers) =>
            {
                //TODO 根据userId 来选择节点
                return servercallInvokers[0];
            });
        }


        /// <summary>
        /// 扩展测试二，明确
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Service.Grpc.GrpcExampleService.GrpcExampleServiceClient Client(this IGrpcClient<Service.Grpc.GrpcExampleService.GrpcExampleServiceClient> data, long userId)
        {
             return data.CreateClient((servercallInvokers) =>
             {
                 //TODO 根据userId 来选择节点
                 return servercallInvokers[0];
             });
        }
    }
}
