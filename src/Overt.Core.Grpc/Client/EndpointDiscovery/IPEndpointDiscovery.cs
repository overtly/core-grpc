using System;
using System.Collections.Generic;
using System.Linq;

namespace Overt.Core.Grpc
{
    /// <summary>
    /// IP服务发现
    /// </summary>
    internal class IPEndpointDiscovery : IEndpointDiscovery
    {
        private readonly List<Tuple<string, int>> _ipEndPoints;
        public IPEndpointDiscovery(string serviceName, List<Tuple<string, int>> ipEndPoints)
        {
            if ((ipEndPoints?.Count ?? 0) <= 0)
                throw new ArgumentNullException("no ip endpoints availble");

            _ipEndPoints = ipEndPoints;
            ServiceName = serviceName;
        }

        public string ServiceName { get; set; }

        public List<string> FindServiceEndpoints(bool filterBlack = true)
        {
            if ((_ipEndPoints?.Count ?? 0) <= 0)
                throw new ArgumentOutOfRangeException("endpoint not provide");

            var targets = _ipEndPoints.Select(x => $"{x.Item1}:{x.Item2}")
                                      .Where(target => !ServiceBlackPlicy.In(ServiceName, target) || !filterBlack)
                                      .ToList();
            return targets;
        }
    }
}
