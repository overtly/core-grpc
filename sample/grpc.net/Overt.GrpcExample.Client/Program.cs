using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Overt.Core.Grpc.H2;
using System.Threading;
using Grpc.Net.Client;
using System.Net.Http;
using static Overt.GrpcExample.Service.Grpc.GrpcExampleService;

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
            services.AddGrpcClient();

            // 第三方配置，启动可用
            //services.AddGrpcConfig(config =>
            //{
            //    config.AddApollo(configuration.GetSection("apollo")).AddDefault();
            //});
            services.Configure<GrpcClientOptions<GrpcExampleServiceClient>>(cfg =>
            {
                cfg.ConfigPath = "";
                cfg.GrpcChannelOptions = new GrpcChannelOptions()
                {
                    HttpHandler = new SocketsHttpHandler()
                    {
                        EnableMultipleHttp2Connections = true,
                    }
                };
            });

            provider = services.BuildServiceProvider();
        }


        static void Main(string[] args)
        {
            do
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.A)
                    break;

                //var httpClientHandler = new HttpClientHandler();
                //httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                //var httpClient = new HttpClient(httpClientHandler);
                //Constants.DefaultChannelOptions.HttpClient = httpClient;
                try
                {
                    var service = provider.GetService<IGrpcClient<Service.Grpc.GrpcExampleService.GrpcExampleServiceClient>>();
                    var client = service.CreateClient((channelWrappers) =>
                    {
                        return channelWrappers[0];
                    });
                    var res = client.Ask(new Service.Grpc.AskRequest() { Key = "abc" });
                    Console.WriteLine(DateTime.Now + " - " + res.Content ?? "abc");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} - 异常");
                }

                Thread.Sleep(200);

            } while (true);

            Console.WriteLine("over");
            Console.ReadLine();
        }
    }
}
