using Microsoft.Extensions.Hosting;
using Overt.Core.Grpc;
using Overt.Core.Grpc.Intercept;
using Overt.GrpcExample.Service.Grpc;
using System.Threading;
using System.Threading.Tasks;

namespace Overt.GrpcExample.Service
{
    public class GrpcExampleHostedService : IHostedService
    {
        readonly IServerTracer _tracer;
        readonly GrpcExampleService.GrpcExampleServiceBase _grpcServiceBase;
        public GrpcExampleHostedService(
            IServerTracer tracer,
            GrpcExampleService.GrpcExampleServiceBase grpcServiceBase)
        {
            _tracer = tracer;
            _grpcServiceBase = grpcServiceBase;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                GrpcServiceManager.Start(GrpcExampleService.BindService(_grpcServiceBase));
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                GrpcServiceManager.Stop();
            }, cancellationToken);
        }
    }
}
