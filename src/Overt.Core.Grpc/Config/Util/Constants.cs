using Grpc.Core;
using System.Collections.Generic;

namespace Overt.Core.Grpc
{
    public class Constants
    {
#if !ASP_NET_CORE
        /// <summary>
        /// Grpc Server 节点名称
        /// </summary>
        internal const string GrpcServerSectionName = "grpcServer";
        /// <summary>
        /// Grpc Consul 节点名称
        /// </summary>
        internal const string ConsulServerSectionName = "consulServer";
        /// <summary>
        /// Grpc Client 节点名称
        /// </summary>
        internal const string GrpcClientSectionName = "grpcClient";
#else
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
#endif

        /// <summary>
        /// 
        /// </summary>
        //public const string MetadataKey_ChannelTarget = "channel_target";

        /// <summary>
        /// 默认的通道配置
        /// </summary>
        public static List<ChannelOption> DefaultChannelOptions = new List<ChannelOption>()
        {
            new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue),
            new ChannelOption(ChannelOptions.MaxSendMessageLength, int.MaxValue),
        };

    }
}
