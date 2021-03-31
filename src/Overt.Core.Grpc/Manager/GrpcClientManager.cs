using Grpc.Core;
using Grpc.Core.Interceptors;
using Overt.Core.Grpc.Intercept;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Overt.Core.Grpc
{
#if !ASP_NET_CORE
    /// <summary>
    /// Grpc客户端管理类
    /// </summary>
    public class GrpcClientManager<T> where T : ClientBase
    {
        static readonly ConcurrentDictionary<Type, GrpcClientFactory<T>> _clientFactoryCache = new ConcurrentDictionary<Type, GrpcClientFactory<T>>();
        static readonly ConcurrentDictionary<Type, T> _clientCache = new ConcurrentDictionary<Type, T>();
        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="options">配置信息</param>
        /// <param name="callInvokers">自定义invoker获取策略</param>
        /// <returns></returns>
        public static T Get(string configPath = "", GrpcClientOptions options = default, Func<List<ServerCallInvoker>, ServerCallInvoker> callInvokers = null)
        {
            options = options ?? new GrpcClientOptions();
            if (!string.IsNullOrWhiteSpace(configPath))
                options.ConfigPath = configPath;

            return Get(options, callInvokers);
        }

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="tracer">tracer拦截器</param>
        /// <param name="interceptors">自定义拦截器</param>
        /// <param name="callInvokers">自定义invoker获取策略</param>
        /// <returns></returns>
        public static T Get(string configPath = "", IClientTracer tracer = null, List<Interceptor> interceptors = null, Func<List<ServerCallInvoker>, ServerCallInvoker> callInvokers = null)
        {
            var options = new GrpcClientOptions()
            {
                Tracer = tracer,
                ConfigPath = configPath,
            };
            if (interceptors?.Count > 0)
                options.Interceptors.AddRange(interceptors);
            return Get(options, callInvokers);
        }

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="tracer">tracer拦截器</param>
        /// <param name="interceptors">自定义拦截器</param>
        /// <param name="callInvokers">自定义invoker获取策略</param>
        /// <returns></returns>
        public static T Get(GrpcClientOptions options, Func<List<ServerCallInvoker>, ServerCallInvoker> callInvokers = null)
        {
            var factory = _clientFactoryCache.GetOrAdd(typeof(T), key => new GrpcClientFactory<T>(options));
            return _clientCache.GetOrAdd(typeof(T), key => factory.Get(callInvokers));
        }
    }
#endif
}
