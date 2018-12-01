using Grpc.Core;
using Sodao.Core.Grpc.Intercept;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sodao.Core.Grpc
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
        T Get(string configFile = "");
    }
}
