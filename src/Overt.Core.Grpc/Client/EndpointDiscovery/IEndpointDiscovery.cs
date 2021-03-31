using System;
using System.Collections.Generic;

namespace Overt.Core.Grpc
{
    /// <summary>
    /// 重点服务发现接口
    /// </summary>
    public interface IEndpointDiscovery
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        GrpcClientOptions Options { get; set; }

        /// <summary>
        /// 监听变动的方法
        /// </summary>
        Action Watched { get; set; }

        /// <summary>
        /// 获取服务可连接终结点
        /// </summary>
        /// <returns></returns>
        List<Tuple<string, string>> FindServiceEndpoints(bool filterBlack = true);
    }
}
