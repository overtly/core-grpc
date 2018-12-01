using Grpc.Core;

namespace Sodao.Core.Grpc
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
    }
}
