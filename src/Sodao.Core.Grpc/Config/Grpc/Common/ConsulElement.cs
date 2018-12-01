#if NET45 || NET46 || NET47
using System.Configuration;
#endif

namespace Sodao.Core.Grpc
{
    /// <summary>
    /// Consul
    /// </summary>
    public class ConsulElement
#if NET45 || NET46 || NET47
        : ConfigurationElement
#endif
    {
        /// <summary>
        /// 路径
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("path", IsRequired = false)]
#endif
        public string Path
        {
#if NET45 || NET46 || NET47
            get { return (string)this["path"]; }
#else
        get; set;
#endif
        }
    }
}
