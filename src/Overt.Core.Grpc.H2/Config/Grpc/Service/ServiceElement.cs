namespace Overt.Core.Grpc.H2.Service
{
    public class ServiceElement
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 注册配置
        /// </summary>
        public ConsulElement Consul { get; set; }
    }
}
