using System;
using System.Collections.Generic;
using System.Linq;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// IP服务发现
    /// </summary>
    internal class IPEndpointDiscovery : IEndpointDiscovery
    {
        #region Constructor
        private readonly List<Tuple<string, int>> _ipEndPoints;
        public IPEndpointDiscovery(GrpcClientOptions options, List<Tuple<string, int>> ipEndPoints)
        {
            if ((ipEndPoints?.Count ?? 0) <= 0)
                throw new ArgumentNullException("no ip endpoints availble");

            _ipEndPoints = ipEndPoints;

            Options = options;
        }
        #endregion

        #region Public Property
        public GrpcClientOptions Options { get; set; }

        public Action Watched { get; set; }
        #endregion

        #region Public Method
        public List<(string serviceId, string target)> FindServiceEndpoints(bool filterBlack = true)
        {
            if ((_ipEndPoints?.Count ?? 0) <= 0)
                throw new ArgumentOutOfRangeException("endpoint not provide");

            var targets = _ipEndPoints.Select(x => ("", $"{x.Item1}:{x.Item2}"))
                                      .Where(target => !ServiceBlackPolicy.In(Options.ServiceName, target.Item2) || !filterBlack)
                                      .ToList();
            return targets;
        }
        #endregion
    }
}
