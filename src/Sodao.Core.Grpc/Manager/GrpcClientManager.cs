using Grpc.Core;
using Sodao.Core.Grpc.Intercept;
using System;
using System.Collections.Concurrent;
#if !ASP_NET_CORE
using System.Configuration;
using System.Linq;
#endif

namespace Sodao.Core.Grpc
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
        /// <param name="configPath"></param>
        /// <param name="tracer"></param>
        /// <returns></returns>
        public static T Get(string configPath = "", IClientTracer tracer = null)
        {
            var factory = new GrpcClientFactory<T>(tracer);
            return _clientCache.GetOrAdd(typeof(T), key => factory.Get(configPath));
        }
    }

    /// <summary>
    /// Grpc客户端管理类原来的
    /// </summary>
    [Obsolete("use GrpcClientManager<T>")]
    public class GrpcClientManager
    {
        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static T Get<T>(string sectionName = "grpcClient", string configPath = "")
            where T : ClientBase
        {
            var factory = new GrpcClientFactory<T>();
            return factory.Get(configPath);
        }
    }
#endif
}
