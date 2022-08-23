using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 静态
    /// </summary>
    public class ClientUtil
    {
        private readonly static object _lockHelper = new object();
        private readonly static ConcurrentDictionary<string, Exitus> _exitusMap = new ConcurrentDictionary<string, Exitus>();
        private readonly static ConcurrentDictionary<string, GrpcClientOptions> _options = new ConcurrentDictionary<string, GrpcClientOptions>();

        /// <summary>
        /// 加入缓存
        /// </summary>
        /// <param name="options"></param>
        public static void AddCache(GrpcClientOptions options)
        {
            if (options == null)
                return;

            _options.AddOrUpdate(options.ServiceName, options, (k, v) => options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static Exitus GetExitus(string serviceName)
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

                exitus = ResolveConfiguration(options);
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
        internal static Exitus ResolveConfiguration(GrpcClientOptions options)
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
        internal static GrpcServiceElement ResolveServiceConfiguration(string configFile)
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
        internal static IEndpointStrategy ResolveStickyConfiguration(string address, GrpcClientOptions options)
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
        internal static IEndpointStrategy ResolveEndpointConfiguration(GrpcServiceElement service, GrpcClientOptions options)
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
        internal static bool EnableConsul(GrpcDiscoveryElement discovery, out string address)
        {
            address = string.Empty;
            var configPath = discovery?.Consul?.Path;
            if (string.IsNullOrWhiteSpace(configPath))
                return false;

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, discovery.Consul.Path)))
                throw new Exception($"[{discovery.Consul.Path}] not exist at [{AppDomain.CurrentDomain.BaseDirectory}]");

            var consulSection = ConfigBuilder.Build<ConsulServerSection>(Constants.ConsulServerSectionName, configPath);
            if (string.IsNullOrWhiteSpace(consulSection?.Service?.Address))
                return false;

            address = consulSection?.Service?.Address;
            return true;
        }
        #endregion
    }
}
