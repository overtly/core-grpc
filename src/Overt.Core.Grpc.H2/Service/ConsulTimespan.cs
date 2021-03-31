using System;

namespace Overt.Core.Grpc.H2
{
    internal static class ConsulTimespan
    {
        /// <summary>
        /// 健康检测时间 10s
        /// </summary>
        public static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(10);

        /// <summary>
        /// 定时上报时间 30s
        /// </summary>
        public static readonly TimeSpan SelfCheckInterval = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 移除服务的时效 15s
        /// </summary>
        public static readonly TimeSpan CriticalInterval = TimeSpan.FromSeconds(20);
    }
}