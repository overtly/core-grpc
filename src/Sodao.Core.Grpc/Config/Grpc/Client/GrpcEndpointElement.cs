#if !ASP_NET_CORE
using System.Configuration;
#endif

namespace Sodao.Core.Grpc
{
    /// <summary>
    /// IP地址
    /// </summary>
    public class GrpcEndpointElement
#if !ASP_NET_CORE
        : ConfigurationElement
#endif
    {
        /// <summary>
        /// 服务IP
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("host", IsRequired = true)]
#endif
        public string Host
        {
#if !ASP_NET_CORE
            get { return (string)this["host"]; }
#else
        get; set;
#endif
        }
        /// <summary>
        /// 端口
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("port", IsRequired = true)]
#endif
        public int Port
        {
#if !ASP_NET_CORE
            get { return (int)this["port"]; }
#else
        get; set;
#endif
        }

        /// <summary>
        /// to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(this.Host, ":", this.Port.ToString());
        }
    }
}
