#if NET5_0_OR_GREATER
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 静态注入
    /// </summary>
    public class InternalResolver : PollingResolver
    {
        private readonly Uri _address;
        private readonly StrategyFactory _strategyFactory;
        public InternalResolver(Uri address, ILoggerFactory loggerFactory, StrategyFactory strategyFactory) : base(loggerFactory)
        {
            _address = address;
            _strategyFactory = strategyFactory;
        }

        protected override Task ResolveAsync(CancellationToken cancellationToken)
        {
            var options = _address.LocalPath.Replace("/", "");
            var exitus = _strategyFactory.GetExitus(options);
            var targets = exitus.EndpointStrategy.GetTargets(exitus.ServiceName);
            if ((targets?.Count ?? 0) <= 0)
            {
                exitus.EndpointStrategy.NodeChanged = () =>
                {
                    Refresh();
                };
                Listener(ResolverResult.ForResult(null));
                return Task.CompletedTask;
            }

            var addresses = new List<BalancerAddress>();
            foreach (var target in targets ?? new List<string>())
            {
                var arr = target.Split(':');
                var host = arr[0];
                var port = 80;
                if (arr.Length == 2 && int.TryParse(arr[1], out int p) && p > 0)
                    port = p;

                addresses.Add(new BalancerAddress(host, port));
            }
            Listener(ResolverResult.ForResult(addresses));
            return Task.CompletedTask;
        }
    }
}
#endif