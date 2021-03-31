using Grpc.Net.Client;
using System.Collections.Generic;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 单例使用
    /// </summary>
    public interface IEndpointStrategy
    {
        /// <summary>
        /// 添加服务发现
        /// </summary>
        /// <param name="serviceDiscovery"></param>
        void AddServiceDiscovery(IEndpointDiscovery serviceDiscovery);

        /// <summary>
        /// 获取所有可用节点
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        List<ChannelWrapper> GetChannelWrappers(string serviceName);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        ChannelWrapper GetChannelWrapper(string serviceName);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="channel"></param>
        void Revoke(string serviceName, ChannelWrapper channel);

        /// <summary>
        /// 定时检测
        /// </summary>
        void InitCheckTimer();
    }
}