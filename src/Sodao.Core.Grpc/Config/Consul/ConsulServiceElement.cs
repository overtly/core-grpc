#if NET45 || NET46 || NET47
using System.Configuration;
#endif

namespace Sodao.Core.Grpc
{
    /// <summary>
    /// 
    /// </summary>
    public class ConsulServiceElement
#if NET45 || NET46 || NET47
        : ConfigurationElement
#endif
    {
        /// <summary>
        /// consul地址
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("address", IsRequired = true)]
#endif
        public string Address
        {
#if NET45 || NET46 || NET47
            get { return (string)this["address"]; }
#else
            get;set;
#endif
        }
    }
}
