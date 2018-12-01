#if NET45 || NET46 || NET47
using System.Configuration;
#endif

namespace Sodao.Core.Grpc
{
    /// <summary>
    /// IP地址
    /// </summary>
    public class GrpcEndpointElement
#if NET45 || NET46 || NET47
        : ConfigurationElement
#endif
    {
        /// <summary>
        /// 服务IP
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("host", IsRequired = true)]
#endif
        public string Host
        {
#if NET45 || NET46 || NET47
            get { return (string)this["host"]; }
#else
        get; set;
#endif
        }
        /// <summary>
        /// 端口
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("port", IsRequired = true)]
#endif
        public int Port
        {
#if NET45 || NET46 || NET47
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
