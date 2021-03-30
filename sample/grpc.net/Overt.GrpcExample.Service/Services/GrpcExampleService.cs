using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Overt.GrpcExample.Service.Grpc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Overt.GrpcExample.Service
{
    public class GrpcExampleServiceImpl : Grpc.GrpcExampleService.GrpcExampleServiceBase
    {
        IServiceProvider _provider;
        IConfiguration _configuration;
        public GrpcExampleServiceImpl(IServiceProvider provider, IConfiguration configuration)
        {
            _provider = provider;
            _configuration = configuration;
        }

        public override Task<ResponseModel> GetName(RequestModel request, ServerCallContext context)
        {
            ResponseModel model = null;
            if (request == null || request.Key == null)
                return Task.FromResult(model);

            model = new ResponseModel() { };
            return Task.FromResult(model);
        }

        public override Task<Grpc.AskResponse> Ask(Grpc.AskRequest request, ServerCallContext context)
        {
            var v = _configuration.GetSection("GrpcServer").GetSection("Service").GetValue<int>("Port");

            var model = new AskResponse() { Content = $"Ask: {request.Key}: {DateTime.Now.ToString()} -- configuration: {v} --- address: {string.Join(",", Dns.GetHostEntry(Dns.GetHostName()).AddressList.Select(oo => oo.ToString()))}" };
            return Task.FromResult(model);
        }
    }
}
