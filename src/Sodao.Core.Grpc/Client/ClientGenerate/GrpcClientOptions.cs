using Grpc.Core;

namespace Sodao.Core.Grpc
{
    /// <summary>
    /// 客户端配置
    /// </summary>
    public class GrpcClientOptions<T> where T : ClientBase
    {
        /// <summary>
        /// Json文件
        /// defaultValue: dllconfigs/clientsettings.json 
        /// </summary>
        public string JsonFile { get; set; }
    }
}
