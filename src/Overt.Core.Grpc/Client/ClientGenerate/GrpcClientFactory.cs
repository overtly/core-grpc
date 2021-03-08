using Grpc.Core;
using Grpc.Core.Interceptors;
using Overt.Core.Grpc.Intercept;
using System.Collections.Generic;
#if ASP_NET_CORE
using Microsoft.Extensions.Options;
#endif
using System;
using System.Linq;

namespace Overt.Core.Grpc
{
    /// <summary>
    /// 客户端工厂
    /// </summary>
    public class GrpcClientFactory<T> : IGrpcClientFactory<T> where T : ClientBase
    {
        private readonly IClientTracer _tracer;
        private readonly List<Interceptor> _interceptors;

#if ASP_NET_CORE
        private readonly GrpcClientOptions<T> _options;

        public GrpcClientFactory(IOptions<GrpcClientOptions<T>> options = null, IClientTracer tracer = null, IOptions<GrpcClientOptions> grpcOptions = null)
        {
            _options = options?.Value;
            _tracer = tracer;
            _interceptors = grpcOptions?.Value?.Interceptors;
        }
#else
        public GrpcClientFactory(IClientTracer tracer = null, List<Interceptor> interceptors = null)
        {
            _tracer = tracer;
            _interceptors = interceptors;
        }
#endif

        /// <summary>
        /// 构造实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get(string configPath = "")
        {
            var _callInvoker = GetCallInvoker(configPath);
            var client = (T)Activator.CreateInstance(typeof(T), _callInvoker);
            return client;
        }

        #region Private Method
        /// <summary>
        /// 获取CallInvoker
        /// </summary>
        /// <returns></returns>
        private ClientCallInvoker GetCallInvoker(string configPath = "")
        {
            var exitus = StrategyFactory.Get<T>(GetConfigPath(configPath));
            var callInvoker = new ClientCallInvoker(exitus.EndpointStrategy, exitus.ServiceName, exitus.MaxRetry, _tracer, _interceptors);
            return callInvoker;
        }

        /// <summary>
        /// 获取命名空间
        /// </summary>
        /// <returns></returns>
        private string GetConfigPath(string configPath = "")
        {
#if ASP_NET_CORE
            if (string.IsNullOrEmpty(configPath))
                configPath = _options?.ConfigPath;
#endif
            if (string.IsNullOrEmpty(configPath))
                configPath = $"dllconfigs/{typeof(T).Namespace}.dll.json";

            return configPath;
        }
        #endregion
    }
}