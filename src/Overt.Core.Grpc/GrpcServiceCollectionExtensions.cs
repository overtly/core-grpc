#if ASP_NET_CORE
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Overt.Core.Grpc.Intercept;
using System;
#endif

namespace Overt.Core.Grpc
{
#if ASP_NET_CORE
    /// <summary>
    /// Grpc服务注入
    /// </summary>
    public static class GrpcServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcClient(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAdd(ServiceDescriptor.Singleton(typeof(IGrpcClient<>), typeof(GrpcClient<>)));
            services.TryAdd(ServiceDescriptor.Singleton(typeof(IGrpcClientFactory<>), typeof(GrpcClientFactory<>)));
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcTracer<T>(this IServiceCollection services) where T : IServerTracer
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddTransient(typeof(IServerTracer), typeof(T));
            return services;
        }

        /// <summary>
        /// 配置 可用于第三方配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureDelegate"></param>
        public static void AddGrpcConfig(this IServiceCollection services, Action<IConfigurationBuilder> configureDelegate)
        {
            ConfigBuilder.ConfigureDelegate = configureDelegate;
        }
    }
#endif
}