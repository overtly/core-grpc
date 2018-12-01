using Grpc.Core;
using Sodao.Core.Grpc.Intercept;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sodao.GrpcExampleNet45.Service.Tracer
{
    public class ConsoleTracer : IServerTracer
    {
        public string ServiceName { get; set; }

        public void Exception<TRequest>(ServerCallContext context, Exception exception, TRequest request = default(TRequest))
        {
            Console.WriteLine("some exception");
        }

        public void Finish(ServerCallContext context)
        {
            Console.WriteLine("finished request");
        }

        public void Request<TRequest>(TRequest request, ServerCallContext context)
        {
            Console.WriteLine("start request");
        }

        public void Response<TResponse>(TResponse response, ServerCallContext context)
        {
            Console.WriteLine("end response");
        }
    }
}
