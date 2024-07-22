#if !ASP_NET_CORE
using System.Configuration;
#endif

namespace Overt.Core.Grpc
{
    /// <summary>
    /// 
    /// </summary>
    public class ConsulServiceElement
#if !ASP_NET_CORE
        : ConfigurationElement
#endif
    {
        /// <summary>
        /// consul地址
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("address", IsRequired = true)]
#endif
        public string Address
        {
#if !ASP_NET_CORE
            get { return (string)this["address"]; }
#else
            get;set;
#endif
        }
        /// <summary>
        /// consul token
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("token", IsRequired = false)]
#endif
        public string Token
        {
#if !ASP_NET_CORE
            get { return (string)this["token"]; }
#else
            get;set;
#endif
        }
    }
}
