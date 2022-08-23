#if NET5_0_OR_GREATER
using Grpc.Net.Client.Balancer;

namespace Overt.Core.Grpc.H2
{
    public class RandomBalancerFactory : LoadBalancerFactory
    {
        public override string Name => "random";

        public override LoadBalancer Create(LoadBalancerOptions options)
        {
            return new RandomBalancer(options.Controller, options.LoggerFactory);
        }
    }
}
#endif