using System.Collections.Generic;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 服务发现
    /// </summary>
    public class GrpcDiscoveryElement
    {
        /// <summary>
        /// 服务器集合。
        /// </summary>
        public List<GrpcEndpointElement> EndPoints { get; set; }

        /// <summary>
        /// 注册配置
        /// </summary>
        public ConsulElement Consul { get; set; }
    }
}