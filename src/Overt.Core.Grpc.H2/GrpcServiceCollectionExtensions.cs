using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// Grpc服务注入
    /// </summary>
    public static class GrpcServiceCollectionExtensions
    {
        #region 客户端
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcClient(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.Add(ServiceDescriptor.Singleton(typeof(IGrpcClient<>), typeof(GrpcClient<>)));
            services.Add(ServiceDescriptor.Singleton(typeof(IGrpcClientFactory<>), typeof(GrpcClientFactory<>)));
            return services;
        }

        /// <summary>
        /// 配置 可用于第三方配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureDelegate"></param>
        public static IServiceCollection AddGrpcConfig(this IServiceCollection services, Action<IConfigurationBuilder> configureDelegate)
        {
            ConfigBuilder.ConfigureDelegate = configureDelegate;
            return services;
        }
        #endregion

        #region 服务端
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcService(this IServiceCollection services)
        {
            services.AddSingleton<IHostedService, GrpcHostedService>();
            return services;
        }

        /// <summary>
        /// 服务注册
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseGrpcRegister(this IApplicationBuilder app, Action<GrpcOptions> grpcOptionBuilder = null)
        {
            var grpcOptions = new GrpcOptions();
            grpcOptionBuilder?.Invoke(grpcOptions);

            var serverAddressFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            if (serverAddressFeature?.Addresses?.Count > 0)
                grpcOptions.ListenAddress = serverAddressFeature.Addresses.FirstOrDefault();

            RegisterFactory.WithConsul(grpcOptions);
            return app;
        }
        #endregion
    }
}