using Grpc.Core;
using System;
using System.Collections.Generic;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 接口类
    /// </summary>
    public interface IGrpcClient<T> where T : ClientBase
    {
        /// <summary>
        /// 单例对象
        /// </summary>
        T Client { get; }

        /// <summary>
        /// 每次构造一个新的对象
        /// </summary>
        /// <param name="getInvoker"></param>
        /// <returns></returns>
        T CreateClient(Func<List<ChannelWrapper>, ChannelWrapper> getChannel);
    }
}
