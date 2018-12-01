using Autofac;
using Grpc.Core;
using Sodao.GrpcExample.Service.Grpc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Sodao.GrpcExampleNet45.Service
{
    public class GrpcExampleServiceImpl : GrpcExampleService.GrpcExampleServiceBase
    {
        #region Private Fields
        private readonly IContainer _container;
        #endregion

        #region Constructor

        public GrpcExampleServiceImpl(IContainer container)
        {
            _container = container;
        }
        #endregion

        public override Task<ResponseModel> GetName(RequestModel request, ServerCallContext context)
        {
            ResponseModel model = null;
            if (request == null || request.Key == null)
                return Task.FromResult(model);

            model = new ResponseModel() { };
            return Task.FromResult(model);
        }

        public override Task<AskResponse> Ask(AskRequest request, ServerCallContext context)
        {
            var model = new AskResponse() { Content = $"Ask: {request.Key}: {DateTime.Now.ToString()} --- address: {string.Join(",", Dns.GetHostEntry(Dns.GetHostName()).AddressList.Select(oo => oo.ToString()))}" };
            return Task.FromResult(model);
        }
    }
}
