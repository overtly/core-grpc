using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Overt.Core.Grpc
{
    internal class ServicePollingPolicy
    {
        static private long _times = 0;

        /// <summary>
        /// 轮询策略
        /// </summary>
        /// <param name="callInvokers"></param>
        /// <returns></returns>
        public static ServerCallInvoker Random(List<ServerCallInvoker> callInvokers)
        {
            if ((callInvokers?.Count ?? 0) <= 0)
                return null;

            return callInvokers[(int)(Interlocked.Increment(ref _times) % callInvokers.Count)];
        }
    }
}
