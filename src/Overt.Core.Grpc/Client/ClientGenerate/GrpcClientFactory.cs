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
        private readonly GrpcClientOptions _globalOptions;

#if ASP_NET_CORE
        private readonly GrpcClientOptions<T> _options;

        public GrpcClientFactory(IOptions<GrpcClientOptions<T>> options = null, IOptions<GrpcClientOptions> globalOptions = null)
        {
            _options = options?.Value;
            _globalOptions = globalOptions?.Value ?? new GrpcClientOptions();
            if (_globalOptions.Tracer != null)
                _globalOptions.Interceptors.Add(new ClientTracerInterceptor(_globalOptions.Tracer));
        }
#else
        public GrpcClientFactory(GrpcClientOptions globalOptions = null)
        {
            _globalOptions = globalOptions ?? new GrpcClientOptions();
            if (_globalOptions.Tracer != null)
                _globalOptions.Interceptors.Add(new ClientTracerInterceptor(_globalOptions.Tracer));
        }
#endif

        /// <summary>
        /// 构造实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get(string configPath = "", Func<List<ServerCallInvoker>, ServerCallInvoker> getInvoker = null)
        {
            var exitus = StrategyFactory.Get<T>(GetConfigPath(configPath));
            _globalOptions.ServiceName = exitus.ServiceName;
            _globalOptions.MaxRetry = exitus.MaxRetry;
            _globalOptions.GetInvoker = getInvoker;
            var callInvoker = new ClientCallInvoker(exitus.EndpointStrategy, _globalOptions);
            var client = (T)Activator.CreateInstance(typeof(T), callInvoker);
            return client;
        }

        #region Private Method
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