using Grpc.Core;
using Microsoft.Extensions.Options;
using System;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 客户端工厂
    /// </summary>
    public class GrpcClientFactory<T> : IGrpcClientFactory<T> where T : ClientBase
    {
        private readonly GrpcClientOptions<T> _options;

        public GrpcClientFactory(IOptions<GrpcClientOptions<T>> options = null)
        {
            _options = options?.Value;
        }

        /// <summary>
        /// 构造实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get(string configPath = "")
        {
            var exitus = StrategyFactory.Get<T>(GetConfigPath(configPath));
            var channel = exitus.EndpointStrategy.GetChannel(exitus.ServiceName);
            var client = (T)Activator.CreateInstance(typeof(T), channel);
            return client;
        }

        #region Private Method
        /// <summary>
        /// 获取命名空间
        /// </summary>
        /// <returns></returns>
        private string GetConfigPath(string configPath = "")
        {
            if (string.IsNullOrEmpty(configPath))
                configPath = _options?.ConfigPath;

            if (string.IsNullOrEmpty(configPath))
                configPath = $"dllconfigs/{typeof(T).Namespace}.dll.json";

            return configPath;
        }
        #endregion
    }
}