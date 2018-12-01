using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sodao.Core.Grpc;
using Sodao.GrpcExample.Client.Tracer;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sodao.GrpcExample.Client
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

            provider = services.BuildServiceProvider();
        }

        static void Main(string[] args)
        {
            //var client = provider.GetService<IGrpcClient<GrpcExampleServiceClient>>();
            //var res = client.Client.Ask(new Service.Grpc.AskRequest() { Key = "abc" });

            //Parallel.For(0, 1000000, (index) =>
            //{
            //    var client = provider.GetService<IGrpcClient<Sodao.Juketool.WeChat.CloudStorage.Service.Grpc.CloudStorageService.CloudStorageServiceClient>>();
            //    var res = client.Client.Device_GetByIMEI(new Juketool.WeChat.CloudStorage.Service.Grpc.DeviceIMEIRequest()
            //    {
            //        IMEI = "869319020053033"
            //    });
            //    Console.WriteLine(index + " - " + res?.Data?.IMEI + " - " + DateTime.Now);
            //});
            //Console.WriteLine("over.............");
            //return;

            do
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.A)
                    break;

                try
                {
                    //var client = provider.GetService<IGrpcClient<Sodao.Juketool.ClientProxy.Service.Grpc.JuketoolProxyService.JuketoolProxyServiceClient>>();
                    //var res = client.Client.SupplierLogin(new Juketool.ClientProxy.Service.Grpc.SupplierLoginRequest()
                    //{
                    //    UserName = "u100892",
                    //    Password = "123456"
                    //});
                    //Console.WriteLine(res?.Data?.UserId + " - " + DateTime.Now);

                    //var client1 = provider.GetService<IGrpcClient<Sodao.Juketool.WeChat.CloudStorage.Service.Grpc.CloudStorageService.CloudStorageServiceClient>>();
                    //var res1 = client1.Client.Device_GetByIMEI(new Juketool.WeChat.CloudStorage.Service.Grpc.DeviceIMEIRequest()
                    //{
                    //    IMEI = "869319020053033"
                    //});
                    //Console.WriteLine(res1?.Data?.IMEI + " - " + DateTime.Now);

                    //Console.WriteLine($"{DateTime.Now} - Start");
                    var client = provider.GetService<IGrpcClient<Service.Grpc.GrpcExampleService.GrpcExampleServiceClient>>();
                    var res = client.Client.Ask(new Service.Grpc.AskRequest() { Key = "abc" });
                    Console.WriteLine(DateTime.Now + " - " + res.Content ?? "abc");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} - 异常");
                }

                Thread.Sleep(200);

            } while (true);

            //var tasks = new List<Task>();
            //for (int i = 0; i < 1; i++)
            //{
            //    var task = Task.Run(() =>
            //    {
            //        var client = provider.GetService<IGrpcClient<GrpcExampleServiceClient>>();
            //        var res = client.Client.Ask(new Service.Grpc.AskRequest() { Key = "abc" });
            //        Console.WriteLine(res.Content ?? "abc");
            //    });
            //    tasks.Add(task);
            //}

            //Task.WaitAll(tasks.ToArray());
            Console.WriteLine("over");
            Console.ReadLine();
        }
    }
}
