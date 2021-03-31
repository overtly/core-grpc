using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Overt.Core.Grpc.H2
{
    public class GrpcHostedService : IHostedService
    {
        ILogger _logger;
        public GrpcHostedService(ILogger<GrpcHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                RegisterFactory.RemoveConsul(ex =>
                {
                    _logger.LogError(ex, $"Overt.Core.Grpc.H2 移除注册异常");
                });
            });
        }
    }
}
