using System;
using System.Collections.Concurrent;

namespace Sodao.Core.Grpc
{
    /// <summary>
    /// 服务黑名单策略
    /// </summary>
    public class ServiceBlackPlicy
    {
        public readonly static ConcurrentDictionary<string, DateTime> _blacklist = new ConcurrentDictionary<string, DateTime>();

        /// <summary>
        /// 黑名单
        /// </summary>
        /// <param name="target"></param>
        public static void Add(string target)
        {
            var now = DateTime.UtcNow;
            _blacklist.AddOrUpdate(target, k => now, (k, old) => now);
        }

        /// <summary>
        /// 是否在黑名单
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool In(string target)
        {
            if (_blacklist.TryGetValue(target, out DateTime lastFailure))
            {
                // within blacklist period ?
                if (DateTime.UtcNow - lastFailure < ClientTimespan.BlacklistPeriod)
                    return true;

                _blacklist.TryRemove(target, out lastFailure);
            }

            return false;
        }
    }
}
