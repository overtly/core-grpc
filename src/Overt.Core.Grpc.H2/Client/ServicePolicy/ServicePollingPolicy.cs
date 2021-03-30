using Grpc.Net.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Overt.Core.Grpc.H2
{
    internal class ServicePollingPolicy
    {
        static private long _times = 0;

        /// <summary>
        /// 轮询策略
        /// </summary>
        /// <param name="channels"></param>
        /// <returns></returns>
        public static GrpcChannel Random(List<GrpcChannel> channels)
        {
            if ((channels?.Count ?? 0) <= 0)
                return null;

            return channels[(int)(Interlocked.Increment(ref _times) % channels.Count)];
        }
    }
}
