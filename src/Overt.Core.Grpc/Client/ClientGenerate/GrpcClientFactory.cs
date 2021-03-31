using Grpc.Core;
using System.Collections.Generic;
using Overt.Core.Grpc.Intercept;
#if ASP_NET_CORE
using Microsoft.Extensions.Options;
#endif
using System;

namespace Overt.Core.Grpc
{
    /// <summary>
    /// 客户端工厂
    /// </summary>
    public class GrpcClientFactory<T> : IGrpcClientFactory<T> where T : ClientBase
    {
        private GrpcClientOptions<T> _options;

#if ASP_NET_CORE
        public GrpcClientFactory(IOptions<GrpcClientOptions<T>> options)
        {
            _options = options?.Value;
            InitOptions();
        }
#else
        public GrpcClientFactory(GrpcClientOptions options)
        {
            _options = new GrpcClientOptions<T>(options);
            InitOptions();
        }
#endif

        /// <summary>
        /// 构造实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get(Func<List<ServerCallInvoker>, ServerCallInvoker> callInvokers = null)
        {
            var exitus = StrategyFactory.Get<T>(_options);
            var callInvoker = new ClientCallInvoker(_options, exitus.EndpointStrategy, callInvokers);
            var client = (T)Activator.CreateInstance(typeof(T), callInvoker);
            return client;
        }

        #region Private Method
        /// <summary>
        /// 初始化配置
        /// </summary>
        private void InitOptions()
        {
            _options = _options ?? new GrpcClientOptions<T>();
            _options.ConfigPath = GetConfigPath(_options.ConfigPath);
            if (_options.Tracer != null)
                _options.Interceptors.Add(new ClientTracerInterceptor(_options.Tracer));
        }

        /// <summary>
        /// 获取命名空间
        /// </summary>
        /// <returns></returns>
        private string GetConfigPath(string configPath)
        {
#if ASP_NET_CORE
            if (string.IsNullOrEmpty(configPath))
                configPath = $"dllconfigs/{typeof(T).Namespace}.dll.json";
#else
            if (string.IsNullOrEmpty(configPath))
                configPath = $"dllconfigs/{typeof(T).Namespace}.dll.config";
#endif

            return configPath;
        }
        #endregion
    }
}