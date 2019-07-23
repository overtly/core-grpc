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
        /// <summary>
        /// grpc配置
        /// </summary>
#if !ASP_NET_CORE
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
