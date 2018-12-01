using Microsoft.Extensions.Hosting;
using Sodao.Core.Grpc;
using Sodao.Core.Grpc.Intercept;
using Sodao.GrpcExample.Service.Grpc;
using System.Threading;
using System.Threading.Tasks;

namespace Sodao.GrpcExampleCrossPlat.Service
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
                GrpcServiceManager.Start(GrpcExampleService.BindService(_grpcServiceBase), _tracer);
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
