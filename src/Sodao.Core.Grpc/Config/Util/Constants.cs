using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodao.Core.Grpc
{
    public class Constants
    {
#if NET45 || NET46 || NET47
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
