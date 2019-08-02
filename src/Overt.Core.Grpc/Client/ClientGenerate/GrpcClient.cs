using Grpc.Core;
using System;
using System.Collections.Concurrent;

namespace Overt.Core.Grpc
{
    /// <summary>
    /// GrpcClient管理类
    /// </summary>
    public class GrpcClient<T> : IGrpcClient<T> where T : ClientBase
    {
        readonly IGrpcClientFactory<T> _factory;
        readonly ConcurrentDictionary<Type, T> _clientCache = new ConcurrentDictionary<Type, T>();
        public GrpcClient(IGrpcClientFactory<T> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// 获取
        /// </summary>
        public T Client
        {
            get
            {
                return _clientCache.GetOrAdd(typeof(T), key => _factory.Get());
            }
        }
    }
}