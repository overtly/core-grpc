#if NET5_0_OR_GREATER
using Grpc.Net.Client.Balancer;
using System.Collections.Generic;
using System.Threading;

namespace Overt.Core.Grpc.H2
{
    public class RandomPicker : SubchannelPicker
    {
        static private long _times = 0;
        private readonly IReadOnlyList<Subchannel> _subchannels;

        public RandomPicker(IReadOnlyList<Subchannel> subchannels)
        {
            _subchannels = subchannels;
        }

        public override PickResult Pick(PickContext context)
        {
            var subChannels = _subchannels[(int)(Interlocked.Increment(ref _times) % _subchannels.Count)];
            return PickResult.ForSubchannel(subChannels);
        }
    }
}
#endif