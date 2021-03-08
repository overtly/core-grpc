using Grpc.Core;
using System;
using System.Collections.Generic;

namespace Overt.Core.Grpc
{
    /// <summary>
    /// 抽象类
    /// </summary>
    public interface IGrpcClient<T> where T : ClientBase
    {
        /// <summary>
        /// 单例对象
        /// </summary>
        T Client { get; }

        /// <summary>
        /// 对象
        /// </summary>
        /// <param name="action">自定义策略获取节点方式</param>
        /// <returns></returns>
        T CreateClient(Func<List<ServerCallInvoker>, ServerCallInvoker> action);
    }
}