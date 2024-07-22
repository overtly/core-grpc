using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Overt.Core.Grpc;
using Overt.Core.Grpc.Intercept;
using Overt.GrpcExample.Service.Grpc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Overt.GrpcExample.Service
{
    public class GrpcExampleHostedService : IHostedService
    {
        readonly IServerTracer _tracer;
        readonly GrpcExampleService.GrpcExampleServiceBase _grpcServiceBase;
        IConfiguration _configuration;
        public GrpcExampleHostedService(
            IServerTracer tracer,
            GrpcExampleService.GrpcExampleServiceBase grpcServiceBase,
            IConfiguration configuration)
        {
            _tracer = tracer;
            _grpcServiceBase = grpcServiceBase;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                GrpcServiceManager.Start(GrpcExampleService.BindService(_grpcServiceBase), (options) =>
                {
                    options.ChannelOptions = new List<ChannelOption>();
                });
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
