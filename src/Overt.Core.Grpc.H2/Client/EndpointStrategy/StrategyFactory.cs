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
    public class StrategyFactory
    {
        private readonly IServiceProvider _provider;
        private readonly static object _lockHelper = new();
        private readonly static ConcurrentDictionary<string, Exitus> _exitusMap = new();
        private readonly static ConcurrentDictionary<string, GrpcClientOptions> _options = new();

        public StrategyFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

#if NET5_0_OR_GREATER
        private readonly static ConcurrentDictionary<Type, ChannelBase> _channelMap = new();

        public ChannelBase Get<T>(GrpcClientOptions options)
        {
            if (_channelMap.TryGetValue(typeof(T), out ChannelBase channel))
                return channel;

            lock (_lockHelper)
            {
                if (_channelMap.TryGetValue(typeof(T), out channel))
                    return channel;

                channel = ResolveChannelConfiguration(options);
                _channelMap.AddOrUpdate(typeof(T), channel, (k, v) => channel);
                return channel;
            }

        }

        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        private ChannelBase ResolveChannelConfiguration(GrpcClientOptions options)
        {
            var service = ResolveServiceConfiguration(options.ConfigPath);
            if (string.IsNullOrWhiteSpace(options.ServiceName))
                options.ServiceName = service.Name;
            if (string.IsNullOrWhiteSpace(options.Scheme))
                options.Scheme = service.Scheme;

            options.GrpcChannelOptions ??= Constants.DefaultChannelOptions;

            if (options.GrpcChannelOptions.ServiceProvider == null)
                options.GrpcChannelOptions.ServiceProvider = _provider;

            // 使用默认的
            if ((options.GrpcChannelOptions.ServiceConfig?.LoadBalancingConfigs?.Count ?? 0) <= 0)
            {
                options.GrpcChannelOptions.ServiceConfig = new ServiceConfig();
                options.GrpcChannelOptions.ServiceConfig.LoadBalancingConfigs.Add(new LoadBalancingConfig("random"));
            }

            // 配置加入缓存
            _options.AddOrUpdate(options.ServiceName, options, (k, v) => options);

            // 获取Channel
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

        private readonly static ConcurrentDictionary<Type, Exitus> _exitusTypeMap = new();
        /// <summary>
        /// 获取EndpointStrategy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public Exitus Get<T>(GrpcClientOptions options) where T : ClientBase
        {
            if (_exitusTypeMap.TryGetValue(typeof(T), out Exitus exitus) &&
                exitus?.EndpointStrategy != null)
                return exitus;

            lock (_lockHelper)
            {
                if (_exitusTypeMap.TryGetValue(typeof(T), out exitus) &&
                    exitus?.EndpointStrategy != null)
                    return exitus;

                exitus = ResolveExitusConfiguration(options);
                _exitusTypeMap.AddOrUpdate(typeof(T), exitus, (k, v) => exitus);
                return exitus;
            }
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public Exitus GetExitus(string serviceName)
        {
            if (_exitusMap.TryGetValue(serviceName, out Exitus exitus) &&
                exitus?.EndpointStrategy != null)
                return exitus;

            lock (_lockHelper)
            {
                if (_exitusMap.TryGetValue(serviceName, out exitus) &&
                    exitus?.EndpointStrategy != null)
                    return exitus;

                if (!_options.TryGetValue(serviceName, out GrpcClientOptions options))
                    throw new Exception($"配置异常");

                exitus = ResolveExitusConfiguration(options);
                _exitusMap.AddOrUpdate(serviceName, exitus, (k, v) => exitus);
                return exitus;
            }
        }

        #region Private Method
        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        internal Exitus ResolveExitusConfiguration(GrpcClientOptions options)
        {
            var service = ResolveServiceConfiguration(options.ConfigPath);
            IEndpointStrategy endpointStrategy;
            if (EnableConsul(service.Discovery, out string address))
                endpointStrategy = ResolveStickyConfiguration(address, options);
            else
                endpointStrategy = ResolveEndpointConfiguration(service, options);
            return new Exitus(options.ServiceName, endpointStrategy);
        }

        /// <summary>
        /// 解析服务配置
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        internal GrpcServiceElement ResolveServiceConfiguration(string configFile)
        {
            var grpcSection = ConfigBuilder.Build<GrpcClientSection>(Constants.GrpcClientSectionName, configFile);
            if (grpcSection == null || grpcSection.Service == null)
                throw new ArgumentNullException($"service config error");

            return grpcSection.Service;
        }

        /// <summary>
        /// 解析Consul配置
        /// </summary>
        /// <param name="address"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal IEndpointStrategy ResolveStickyConfiguration(string address, GrpcClientOptions options)
        {
            var stickyEndpointDiscovery = new StickyEndpointDiscovery(options, address);
            EndpointStrategy.Instance.AddServiceDiscovery(stickyEndpointDiscovery);
            return EndpointStrategy.Instance;
        }

        /// <summary>
        /// 解析Endpoint配置
        /// </summary>
        /// <param name="service"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal IEndpointStrategy ResolveEndpointConfiguration(GrpcServiceElement service, GrpcClientOptions options)
        {
            var ipEndPoints = service.Discovery.EndPoints.Select(oo => Tuple.Create(oo.Host, oo.Port)).ToList();
            var iPEndpointDiscovery = new IPEndpointDiscovery(options, ipEndPoints);
            EndpointStrategy.Instance.AddServiceDiscovery(iPEndpointDiscovery);
            return EndpointStrategy.Instance;
        }

        /// <summary>
        /// 是否是使用Consul
        /// </summary>
        /// <param name="discovery"></param>
        /// <returns></returns>
        internal bool EnableConsul(GrpcDiscoveryElement discovery, out string address)
        {
            address = string.Empty;
            var configPath = discovery?.Consul?.Path;
            if (string.IsNullOrWhiteSpace(configPath))
                return false;

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, discovery.Consul.Path)))
                throw new Exception($"Cosul配置文件不存在");

            var consulSection = ConfigBuilder.Build<ConsulServerSection>(Constants.ConsulServerSectionName, configPath);
            if (string.IsNullOrWhiteSpace(consulSection?.Service?.Address))
                return false;

            address = consulSection?.Service?.Address;
            return true;
        }
        #endregion
    }
}
