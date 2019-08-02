using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Overt.Core.Grpc;
using Overt.GrpcExample.Service.Grpc;
using Overt.GrpcExampleCrossPlat.Service.Tracer;

namespace Overt.GrpcExampleCrossPlat.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                .UseConsoleLifetime() //使用控制台生命周期
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder
                        .AddJsonFile("appsettings.json", optional: true);    //约定使用appsettings.json作为应用程序配置文件
                })
                .ConfigureServices(ConfigureServices)
                .Build();

            host.Run();
        }

        /// <summary>
        /// 通用DI注入
        /// </summary>
        /// <param name="context"></param>
        /// <param name="services"></param>
        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddTransient<GrpcExampleService.GrpcExampleServiceBase, GrpcExampleServiceImpl>();
            services.AddTransient<IHostedService, GrpcExampleHostedService>();

            // tracer
            services.AddGrpcTracer<ConsoleTracer>();
        }
    }
}
