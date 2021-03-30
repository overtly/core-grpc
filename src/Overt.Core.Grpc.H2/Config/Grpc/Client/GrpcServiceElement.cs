#if !ASP_NET_CORE
using System.Configuration;
#endif

namespace Overt.Core.Grpc.H2.Client
{
    public class GrpcServiceElement
#if !ASP_NET_CORE
        : ConfigurationElement
#endif
    {
        /// <summary>
        /// 服务名称
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("name", IsRequired = true)]
#endif
        public string Name
        {
#if !ASP_NET_CORE
            get { return (string)this["name"]; }
#else
        get; set;
#endif
        }

        /// <summary>
        /// 服务名称
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("maxRetry", IsRequired = false)]
#endif
        public int MaxRetry
        {
#if !ASP_NET_CORE
            get { return (int)this["maxRetry"]; }
#else
        get; set;
#endif
        }

        /// <summary>
        /// 服务发现
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("discovery", IsRequired = true)]
#endif
        public GrpcDiscoveryElement Discovery
        {
#if !ASP_NET_CORE
            get { return this["discovery"] as GrpcDiscoveryElement; }
#else
        get; set;
#endif
        }
    }
}
