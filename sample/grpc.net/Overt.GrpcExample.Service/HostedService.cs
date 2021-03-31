using Microsoft.Extensions.Hosting;
using Overt.Core.Grpc.H2;
using System.Threading;
using System.Threading.Tasks;

namespace Overt.GrpcExample.Service
{
    public class HostedService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {

            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                RegisterFactory.RemoveConsul();
            });
        }
    }
}
