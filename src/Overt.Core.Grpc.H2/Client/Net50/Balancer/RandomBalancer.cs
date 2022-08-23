#if NET5_0_OR_GREATER
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Overt.Core.Grpc.H2
{
    public class RandomBalancer : SubchannelsLoadBalancer
    {
        public RandomBalancer(IChannelControlHelper controller, ILoggerFactory loggerFactory)
            : base(controller, loggerFactory)
        {
        }

        protected override SubchannelPicker CreatePicker(IReadOnlyList<Subchannel> readySubchannels)
        {
            return new RandomPicker(readySubchannels);
        }
    }
}
#endif