#if NETSTANDARD2_0
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sodao.Core.Grpc.Intercept;
using System;
#endif

namespace Sodao.Core.Grpc
{
#if NETSTANDARD2_0
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
        public static IServiceCollection AddGrpcClient<T>(this IServiceCollection services) where T : IClientTracer
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddTransient(typeof(IClientTracer), typeof(T));
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
    }
#endif
}