using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using System;
using System.Collections.Concurrent;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// Endpoint 策略工厂
    /// </summary>
    public class StrategyFactory
    {
        private readonly IServiceProvider _provider;
        private readonly static object _lockHelper = new object();

        public StrategyFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

#if NET5_0_OR_GREATER
        private readonly static ConcurrentDictionary<Type, ChannelBase> _channelMap = new ConcurrentDictionary<Type, ChannelBase>();

        public ChannelBase Get<T>(GrpcClientOptions options)
        {
            if (_channelMap.TryGetValue(typeof(T), out ChannelBase channel))
                return channel;

            lock (_lockHelper)
            {
                if (_channelMap.TryGetValue(typeof(T), out channel))
                    return channel;

                channel = ResolveConfiguration(options);
                _channelMap.AddOrUpdate(typeof(T), channel, (k, v) => channel);
                return channel;
            }

        }

        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        private ChannelBase ResolveConfiguration(GrpcClientOptions options)
        {
            var service = ClientUtil.ResolveServiceConfiguration(options.ConfigPath);
            if (string.IsNullOrWhiteSpace(options.ServiceName))
                options.ServiceName = service.Name;
            if (string.IsNullOrWhiteSpace(options.Scheme))
                options.Scheme = service.Scheme;

            options.GrpcChannelOptions ??= Constants.DefaultChannelOptions;
            options.GrpcChannelOptions.ServiceProvider = _provider;

            // 使用默认的
            if ((options.GrpcChannelOptions.ServiceConfig?.LoadBalancingConfigs?.Count ?? 0) <= 0)
            {
                options.GrpcChannelOptions.ServiceConfig = new ServiceConfig();
                options.GrpcChannelOptions.ServiceConfig.LoadBalancingConfigs.Add(new LoadBalancingConfig("random"));
            }

            ClientUtil.AddCache(options);

            ChannelBase channel;
            // 判断当前使用的
            if (!string.IsNullOrWhiteSpace(options.Scheme))
            {
                channel = GrpcChannel.ForAddress($"{options.Scheme}:///{options.ServiceName}", options.GrpcChannelOptions);
                return channel;
            }


            // 内置的
            channel = GrpcChannel.ForAddress($"internal:///{options.ServiceName}", options.GrpcChannelOptions);
            return channel;
        }
#else

        private readonly static ConcurrentDictionary<Type, Exitus> _exitusMap = new ConcurrentDictionary<Type, Exitus>();
        /// <summary>
        /// 获取EndpointStrategy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public Exitus Get<T>(GrpcClientOptions options) where T : ClientBase
        {
            if (_exitusMap.TryGetValue(typeof(T), out Exitus exitus) &&
                exitus?.EndpointStrategy != null)
                return exitus;

            lock (_lockHelper)
            {
                if (_exitusMap.TryGetValue(typeof(T), out exitus) &&
                    exitus?.EndpointStrategy != null)
                    return exitus;

                exitus = ClientUtil.ResolveConfiguration(options);
                _exitusMap.AddOrUpdate(typeof(T), exitus, (k, v) => exitus);
                return exitus;
            }
        }
#endif
    }
}
