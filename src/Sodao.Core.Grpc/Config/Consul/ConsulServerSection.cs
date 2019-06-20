#if !ASP_NET_CORE
using System.Configuration;
#endif

namespace Sodao.Core.Grpc
{
    /// <summary>
    ///   <consulSettings>
    ///     <service address=""></service>
    ///   </consulSettings>
    /// </summary>
    public class ConsulServerSection
#if !ASP_NET_CORE
        : ConfigurationSection
#endif
    {
#if !ASP_NET_CORE
        /// <summary>
        /// grpc配置
        /// </summary>
        [ConfigurationProperty("service", IsRequired = true)]
#endif
        public ConsulServiceElement Service
        {
#if !ASP_NET_CORE
            get { return this["service"] as ConsulServiceElement; }
#else
            get; set;
#endif

        }
    }
}
