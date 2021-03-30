using Grpc.Core;
using System.Threading.Tasks;

namespace Overt.Core.Grpc.H2
{
    public class ChannelWrapper
    {
        public ChannelWrapper(string serviceId, ChannelBase channel)
        {
            ServiceId = serviceId;
            Channel = channel;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ChannelBase Channel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Target { get { return Channel?.Target; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task ShutdownAsync()
        {
            return Channel?.ShutdownAsync();
        }
    }
}
