#if !ASP_NET_CORE
using System.Configuration;
#endif
using System.Collections.Generic;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 服务发现
    /// </summary>
    public class GrpcDiscoveryElement
#if !ASP_NET_CORE
        : ConfigurationElement
#endif
    {
        /// <summary>
        /// 服务器集合。
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("server", IsRequired = false)]
        public GrpcEndpointElementCollection EndPoints { get { return this["server"] as GrpcEndpointElementCollection; } }
#else
        public List<GrpcEndpointElement> EndPoints { get; set; }
#endif

        /// <summary>
        /// 注册配置
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("consul", IsRequired = false)]
#endif
        public ConsulElement Consul
        {
#if !ASP_NET_CORE
            get { return this["consul"] as ConsulElement; }
#else
        get; set;
#endif
        }
    }
}