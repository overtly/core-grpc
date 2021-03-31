using Grpc.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

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
            _options = options?.Value ?? new GrpcClientOptions<T>();
            _options.ConfigPath = GetConfigPath(_options.ConfigPath);
        }

        /// <summary>
        /// 构造实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get(Func<List<ChannelWrapper>, ChannelWrapper> channelWrapperInvoker = null)
        {
            var exitus = StrategyFactory.Get<T>(_options);

            ChannelWrapper channelWrapper;
            if (channelWrapperInvoker != null)
                channelWrapper = channelWrapperInvoker(exitus.EndpointStrategy.GetChannelWrappers(exitus.ServiceName));
            else
                channelWrapper = exitus.EndpointStrategy.GetChannelWrapper(exitus.ServiceName);


            var client = (T)Activator.CreateInstance(typeof(T), channelWrapper.Channel);
            return client;
        }

        #region Private Method
        /// <summary>
        /// 获取命名空间
        /// </summary>
        /// <returns></returns>
        private string GetConfigPath(string configPath)
        {
            if (string.IsNullOrWhiteSpace(configPath))
                configPath = $"dllconfigs/{typeof(T).Namespace}.dll.json";

            return configPath;
        }
        #endregion
    }
}