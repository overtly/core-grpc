using Grpc.Core;
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

        /// <summary>
        /// 获取EndpointStrategy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static Exitus Get<T>(string configFile)
            where T : ClientBase
        {
            if (_exitusMap.TryGetValue(typeof(T), out Exitus exitus) &&
                exitus?.EndpointStrategy != null)
                return exitus;

            lock (_lockHelper)
            {
                if (_exitusMap.TryGetValue(typeof(T), out exitus) &&
                    exitus?.EndpointStrategy != null)
                    return exitus;

                exitus = ResolveConfiguration(configFile);
                _exitusMap.AddOrUpdate(typeof(T), exitus, (k, v) => exitus);
                return exitus;
            }
        }

        #region Private Method
        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        private static Exitus ResolveConfiguration(string configFile)
        {
            var serviceElement = ResolveServiceConfiguration(configFile);
            var maxRetry = serviceElement.MaxRetry;
            var serviceName = serviceElement.Name;
            var discovery = serviceElement.Discovery;
            IEndpointStrategy endpointStrategy;
            if (EnableConsul(discovery, out string address))
                endpointStrategy = ResolveStickyConfiguration(serviceElement, address);
            else
                endpointStrategy = ResolveEndpointConfiguration(serviceElement);
            return new Exitus(serviceName, maxRetry, endpointStrategy);
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

        /// <summary>
        /// 解析Consul配置
        /// </summary>
        /// <param name="serviceElement"></param>
        /// <returns></returns>
        private static IEndpointStrategy ResolveStickyConfiguration(GrpcServiceElement serviceElement, string address)
        {
            var serviceName = serviceElement.Name;

            // consul
            var stickyEndpointDiscovery = new StickyEndpointDiscovery(serviceName, address);
            EndpointStrategy.Instance.AddServiceDiscovery(stickyEndpointDiscovery);
            return EndpointStrategy.Instance;
        }

        /// <summary>
        /// 解析Endpoint配置
        /// </summary>
        /// <param name="serviceElement"></param>
        /// <returns></returns>
        private static IEndpointStrategy ResolveEndpointConfiguration(GrpcServiceElement serviceElement)
        {
            var serviceName = serviceElement.Name;
            var discovery = serviceElement.Discovery;

            var ipEndPoints = discovery.EndPoints.Select(oo => Tuple.Create(oo.Host, oo.Port)).ToList();
            var iPEndpointDiscovery = new IPEndpointDiscovery(serviceName, ipEndPoints);
            EndpointStrategy.Instance.AddServiceDiscovery(iPEndpointDiscovery);
            return EndpointStrategy.Instance;
        }

        /// <summary>
        /// 是否是使用Consul
        /// </summary>
        /// <param name="discovery"></param>
        /// <returns></returns>
        private static bool EnableConsul(GrpcDiscoveryElement discovery, out string address)
        {
            address = string.Empty;
            var configPath = discovery?.Consul?.Path;
            if (string.IsNullOrEmpty(configPath))
                return false;

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, discovery.Consul.Path)))
                throw new Exception($"[{discovery.Consul.Path}] not exist at [{AppDomain.CurrentDomain.BaseDirectory}]");

            var consulSection = ConfigBuilder.Build<ConsulServerSection>(Constants.ConsulServerSectionName, configPath);
            address = consulSection?.Service?.Address;
            if (string.IsNullOrEmpty(address))
                return false;

            return true;
        }
        #endregion
    }
}
