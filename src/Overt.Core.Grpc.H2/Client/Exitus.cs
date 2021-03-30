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
        /// <param name="maxRetry"></param>
        /// <param name="endpointStrategy"></param>
        public Exitus(string serviceName, int maxRetry, IEndpointStrategy endpointStrategy)
        {
            ServiceName = serviceName;
            MaxRetry = maxRetry;
            EndpointStrategy = endpointStrategy;
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetry { get; set; }
        /// <summary>
        /// 策略
        /// </summary>
        public IEndpointStrategy EndpointStrategy { get; set; }
    }
}
