using Grpc.Net.Client;

namespace Overt.Core.Grpc.H2
{
    public class Constants
    {
        /// <summary>
        /// Grpc Server 节点名称
        /// </summary>
        internal const string GrpcServerSectionName = "GrpcServer";
        /// <summary>
        /// Grpc Consul 节点名称
        /// </summary>
        internal const string ConsulServerSectionName = "ConsulServer";
        /// <summary>
        /// Grpc Client 节点名称
        /// </summary>
        internal const string GrpcClientSectionName = "GrpcClient";

        /// <summary>
        /// 默认的通道配置
        /// </summary>
        public static GrpcChannelOptions DefaultChannelOptions = new GrpcChannelOptions()
        {
            MaxReceiveMessageSize = int.MaxValue,
            MaxSendMessageSize = int.MaxValue,
        };

    }
}
