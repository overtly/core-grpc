#if !ASP_NET_CORE
using System.Configuration;

namespace Sodao.Core.Grpc.Service
{
    /// <summary>
    /// 服务注册
    /// </summary>
    public class GrpcRegistryElement : ConfigurationElement
    {
        /// <summary>
        /// 注册配置
        /// </summary>
        [ConfigurationProperty("consul", IsRequired = false)]
        public ConsulElement Consul { get { return this["consul"] as ConsulElement; } }
    }
}
#endif
