#if !ASP_NET_CORE
using System.Configuration;
#endif

namespace Sodao.Core.Grpc.Service
{
    public class ServiceElement
#if !ASP_NET_CORE
        : ConfigurationElement
#endif
    {
        /// <summary>
        /// 服务名称
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("name", IsRequired = true)]
#endif
        public string Name
        {
#if !ASP_NET_CORE
            get { return (string)this["name"]; }
#else
            get; set;
#endif

        }
        /// <summary>
        /// Host 默认 0.0.0.0
        /// 优先级1
        /// </summary>

#if !ASP_NET_CORE
        [ConfigurationProperty("host", IsRequired = false, DefaultValue = "")]
#else
        public string _Host;
#endif
        public string Host
        {
#if !ASP_NET_CORE
            get { return (string)this["host"]; }
#else
            get { return _Host; }
            set { _Host = value; }
#endif
        }

        /// <summary>
        /// 端口号
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
        /// host环境变量名称
        /// 优先级2
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("hostEnv", IsRequired = false, DefaultValue = "")]
#else
        public string _HostEnv;
#endif
        public string HostEnv
        {
#if !ASP_NET_CORE
            get { return (string)this["hostEnv"]; }
#else
            get { return _HostEnv; }
            set { _HostEnv = value; }
#endif
        }

        /// <summary>
        /// 注册配置
        /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("registry", IsRequired = false)]
        public GrpcRegistryElement Registry { get { return this["registry"] as GrpcRegistryElement; } }
#else
        public ConsulElement Consul { get; set; }
#endif
    }
}
