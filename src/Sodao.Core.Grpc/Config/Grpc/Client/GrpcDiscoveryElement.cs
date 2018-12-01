#if NET45 || NET46 || NET47
using System.Configuration;
#endif
using System.Collections.Generic;

namespace Sodao.Core.Grpc
{
    /// <summary>
    /// 服务发现
    /// </summary>
    public class GrpcDiscoveryElement
#if NET45 || NET46 || NET47
        : ConfigurationElement
#endif
    {
        /// <summary>
        /// 服务器集合。
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("server", IsRequired = false)]
        public GrpcEndpointElementCollection EndPoints { get { return this["server"] as GrpcEndpointElementCollection; } }
#else
        public List<GrpcEndpointElement> EndPoints { get; set; }
#endif

        /// <summary>
        /// 注册配置
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("consul", IsRequired = false)]
#endif
        public ConsulElement Consul
        {
#if NET45 || NET46 || NET47
            get { return this["consul"] as ConsulElement; }
#else
        get; set;
#endif
        }
    }
}