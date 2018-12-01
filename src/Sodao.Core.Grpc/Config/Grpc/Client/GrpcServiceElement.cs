#if NET45 || NET46 || NET47
using System.Configuration;
#endif

namespace Sodao.Core.Grpc.Client
{
    public class GrpcServiceElement
#if NET45 || NET46 || NET47
        : ConfigurationElement
#endif
    {
        /// <summary>
        /// 服务名称
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("name", IsRequired = true)]
#endif
        public string Name
        {
#if NET45 || NET46 || NET47
            get { return (string)this["name"]; }
#else
        get; set;
#endif
        }

        /// <summary>
        /// 服务名称
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("maxRetry", IsRequired = false)]
#endif
        public int MaxRetry
        {
#if NET45 || NET46 || NET47
            get { return (int)this["maxRetry"]; }
#else
        get; set;
#endif
        }

        /// <summary>
        /// 服务发现
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("discovery", IsRequired = true)]
#endif
        public GrpcDiscoveryElement Discovery
        {
#if NET45 || NET46 || NET47
            get { return this["discovery"] as GrpcDiscoveryElement; }
#else
        get; set;
#endif
        }
    }
}
