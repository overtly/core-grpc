namespace Overt.Core.Grpc.H2
{
    public class ServiceElement
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Host 默认 0.0.0.0
        /// 优先级1
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// host环境变量名称
        /// 优先级2
        /// </summary>
        public string HostEnv { get; set; }

        /// <summary>
        /// 注册配置
        /// </summary>
        public ConsulElement Consul { get; set; }
    }
}
