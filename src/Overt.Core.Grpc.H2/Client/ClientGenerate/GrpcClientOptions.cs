using Grpc.Core;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 单服务客户端配置
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
