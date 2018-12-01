#if NET45 || NET46 || NET47
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
#if NET45 || NET46 || NET47
        : ConfigurationSection
#endif
    {
#if NET45 || NET46 || NET47
        /// <summary>
        /// grpc配置
        /// </summary>
        [ConfigurationProperty("service", IsRequired = true)]
#endif
        public ConsulServiceElement Service
        {
#if NET45 || NET46 || NET47
            get { return this["service"] as ConsulServiceElement; }
#else
            get; set;
#endif

        }
    }
}
