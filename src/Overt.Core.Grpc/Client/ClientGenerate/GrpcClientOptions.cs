using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Collections.Generic;

namespace Overt.Core.Grpc
{
    public class GrpcClientOptions
    {
        /// <summary>
        /// Gets a list of <see cref="Interceptor"/> instances used to configure a gRPC client pipeline.
        /// </summary>
        public List<Interceptor> Interceptors { get; } = new List<Interceptor>();
    }

    /// <summary>
    /// 客户端配置
    /// </summary>
    public class GrpcClientOptions<T> where T : ClientBase
    {
        /// <summary>
        /// Json文件
        /// defaultValue: dllconfigs/clientsettings.json 
        /// </summary>
        public string ConfigPath { get; set; }
    }
}
