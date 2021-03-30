namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 出口类
    /// </summary>
    public class Exitus
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="endpointStrategy"></param>
        public Exitus(string serviceName, IEndpointStrategy endpointStrategy)
        {
            ServiceName = serviceName;
            EndpointStrategy = endpointStrategy;
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 策略
        /// </summary>
        public IEndpointStrategy EndpointStrategy { get; set; }
    }
}
