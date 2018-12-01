#if NET45 || NET46 || NET47
using System.Configuration;
#endif

namespace Sodao.Core.Grpc.Service
{
    public class ServiceElement
#if NET45 || NET46 || NET47
        : ConfigurationElement
#endif
    {
        /// <summary>
        /// 服务名称
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("name", IsRequired = true)]
#endif
        public string Name
        {
#if NET45 || NET46 || NET47
            get { return (string)this["name"]; }
#else
            get; set;
#endif

        }
        /// <summary>
        /// Host 默认 0.0.0.0
        /// 优先级1
        /// </summary>

#if NET45 || NET46 || NET47
        [ConfigurationProperty("host", IsRequired = false, DefaultValue = "")]
#else
        public string _Host;
#endif
        public string Host
        {
#if NET45 || NET46 || NET47
            get { return (string)this["host"]; }
#else
            get { return _Host; }
            set { _Host = value; }
#endif
        }

        /// <summary>
        /// 端口号
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
        /// host环境变量名称
        /// 优先级2
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("hostEnv", IsRequired = false, DefaultValue = "")]
#else
        public string _HostEnv;
#endif
        public string HostEnv
        {
#if NET45 || NET46 || NET47
            get { return (string)this["hostEnv"]; }
#else
            get { return _HostEnv; }
            set { _HostEnv = value; }
#endif
        }

        /// <summary>
        /// 注册配置
        /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("registry", IsRequired = false)]
        public GrpcRegistryElement Registry { get { return this["registry"] as GrpcRegistryElement; } }
#else
        public ConsulElement Consul { get; set; }
#endif
    }
}
