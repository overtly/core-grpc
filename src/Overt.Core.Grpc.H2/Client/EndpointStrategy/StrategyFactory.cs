using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// Endpoint 策略工厂
    /// </summary>
    internal class StrategyFactory
    {
        private readonly static object _lockHelper = new object();
        private readonly static ConcurrentDictionary<Type, Exitus> _exitusMap = new ConcurrentDictionary<Type, Exitus>();

#if NET5_0_OR_GREATER
        private readonly static ConcurrentDictionary<Type, ChannelBase> _channelMap = new ConcurrentDictionary<Type, ChannelBase>();

        public static ChannelBase Get<T>(GrpcClientOptions options)
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
        private static ChannelBase ResolveConfiguration(GrpcClientOptions options)
        {
            var service = ResolveServiceConfiguration(options.ConfigPath);
            if (string.IsNullOrWhiteSpace(options.ServiceName))
                options.ServiceName = service.Name;
            if (string.IsNullOrWhiteSpace(options.Scheme))
                options.Scheme = service.Scheme;

            options.GrpcChannelOptions ??= Constants.DefaultChannelOptions;

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

        /// <summary>
        /// 解析服务配置
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        private static GrpcServiceElement ResolveServiceConfiguration(string configFile)
        {
            var grpcSection = ConfigBuilder.Build<GrpcClientSection>(Constants.GrpcClientSectionName, configFile);
            if (grpcSection == null || grpcSection.Service == null)
                throw new ArgumentNullException($"service config error");

            return grpcSection.Service;
        }
#else

        /// <summary>
        /// 获取EndpointStrategy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Exitus Get<T>(GrpcClientOptions options) where T : ClientBase
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
