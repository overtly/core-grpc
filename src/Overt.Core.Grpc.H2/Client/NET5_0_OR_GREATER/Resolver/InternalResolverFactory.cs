#if NET5_0_OR_GREATER
using Grpc.Net.Client.Balancer;

namespace Overt.Core.Grpc.H2
{
    public class InternalResolverFactory : ResolverFactory
    {
        StrategyFactory _strategyFactory;
        public InternalResolverFactory(StrategyFactory strategyFactory)
        {
            _strategyFactory = strategyFactory;
        }

        public override string Name => "internal";

        public override Resolver Create(ResolverOptions options)
        {
            return new InternalResolver(options.Address, options.LoggerFactory, _strategyFactory);
        }
    }
}
#endif