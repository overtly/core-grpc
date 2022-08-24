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
        private readonly Exitus _exitus;
        private readonly StrategyFactory _strategyFactory;
        
        public InternalResolver(Uri address, ILoggerFactory loggerFactory, StrategyFactory strategyFactory) : base(loggerFactory)
        {
            _address = address;
            _strategyFactory = strategyFactory;

            var serviceName = _address.LocalPath.Replace("/", "");
            _exitus = _strategyFactory.GetExitus(serviceName);
            if (_exitus == null)
                throw new Exception($"{serviceName} 配置异常");

            _exitus.EndpointStrategy.NodeChanged = () =>
            {
                Refresh();
            };
        }

        protected override Task ResolveAsync(CancellationToken cancellationToken)
        {
            var targets = _exitus.EndpointStrategy.GetTargets(_exitus.ServiceName);
            if ((targets?.Count ?? 0) <= 0)
            {
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