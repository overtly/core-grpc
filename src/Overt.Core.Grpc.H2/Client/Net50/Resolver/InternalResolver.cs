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
        public InternalResolver(Uri address, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _address = address;
        }

        protected override Task ResolveAsync(CancellationToken cancellationToken)
        {
            var exitus = ClientUtil.GetExitus(_address.LocalPath);
            var targets = exitus.EndpointStrategy.GetTargets(exitus.ServiceName);
            var addresses = new List<BalancerAddress>();
            foreach (var target in targets)
            {
                var address = new BalancerAddress(target.Split(':')[0], int.Parse(target.Split(':')[1]));
                addresses.Add(address);
            }
            Listener(ResolverResult.ForResult(addresses));
            return Task.CompletedTask;
        }
    }
}
#endif