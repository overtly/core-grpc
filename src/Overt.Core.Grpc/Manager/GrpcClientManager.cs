using Grpc.Core;
using Overt.Core.Grpc.Intercept;
using System;
using System.Collections.Concurrent;
#if !ASP_NET_CORE
using System.Configuration;
using System.Linq;
#endif

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
        /// <param name="configPath"></param>
        /// <param name="tracer"></param>
        /// <returns></returns>
        public static T Get(string configPath = "", IClientTracer tracer = null)
        {
            var factory = new GrpcClientFactory<T>(tracer);
            return _clientCache.GetOrAdd(typeof(T), key => factory.Get(configPath));
        }
    }
#endif
}
