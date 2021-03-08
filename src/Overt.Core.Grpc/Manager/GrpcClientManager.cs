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
    public class GrpcClientManager<T>
        where T : ClientBase
    {
        static readonly ConcurrentDictionary<Type, T> _clientCache = new ConcurrentDictionary<Type, T>();
        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="options">配置信息</param>
        /// <returns></returns>
        public static T Get(string configPath = "", GrpcClientOptions options = default)
        {
            var factory = new GrpcClientFactory<T>(options);
            return _clientCache.GetOrAdd(typeof(T), key => factory.Get(configPath));
        }

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="tracer">tracer拦截器</param>
        /// <param name="interceptors">自定义拦截器</param>
        /// <returns></returns>
        public static T Get(string configPath = "", IClientTracer tracer = null, List<Interceptor> interceptors = null)
        {
            var options = new GrpcClientOptions()
            {
                Tracer = tracer,
                Interceptors = interceptors,
            };
            var factory = new GrpcClientFactory<T>(options);
            return _clientCache.GetOrAdd(typeof(T), key => factory.Get(configPath));
        }
    }
#endif
}
