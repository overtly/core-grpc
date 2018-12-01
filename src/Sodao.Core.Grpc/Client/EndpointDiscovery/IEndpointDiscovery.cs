using System.Collections.Generic;

namespace Sodao.Core.Grpc
{
    /// <summary>
    /// 重点服务发现接口
    /// </summary>
    public interface IEndpointDiscovery
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// 获取服务可连接终结点
        /// </summary>
        /// <returns></returns>
        List<string> FindServiceEndpoints(bool filterBlack = true);
    }
}
