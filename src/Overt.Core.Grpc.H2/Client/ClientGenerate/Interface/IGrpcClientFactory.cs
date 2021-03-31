using Grpc.Core;
using System;
using System.Collections.Generic;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 工厂类接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGrpcClientFactory<T> where T : ClientBase
    {
        /// <summary>
        /// 获取Client对象
        /// </summary>
        /// <returns></returns>
        T Get(Func<List<ChannelWrapper>, ChannelWrapper> channelWrapperInvoker = null);
    }
}
