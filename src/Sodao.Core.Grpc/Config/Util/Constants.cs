using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodao.Core.Grpc
{
    internal class Constants
    {
#if !ASP_NET_CORE
        /// <summary>
        /// Grpc Server 节点名称
        /// </summary>
        public const string GrpcServerSectionName = "grpcServer";
        /// <summary>
        /// Grpc Consul 节点名称
        /// </summary>
        public const string ConsulServerSectionName = "consulServer";
        /// <summary>
        /// Grpc Client 节点名称
        /// </summary>
        public const string GrpcClientSectionName = "grpcClient";
#else
        /// <summary>
        /// Grpc Server 节点名称
        /// </summary>
        public const string GrpcServerSectionName = "GrpcServer";
        /// <summary>
        /// Grpc Consul 节点名称
        /// </summary>
        public const string ConsulServerSectionName = "ConsulServer";
        /// <summary>
        /// Grpc Client 节点名称
        /// </summary>
        public const string GrpcClientSectionName = "GrpcClient";
#endif

    }
}
