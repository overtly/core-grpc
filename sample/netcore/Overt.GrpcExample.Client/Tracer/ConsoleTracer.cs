using Grpc.Core;
using Grpc.Core.Interceptors;
using Overt.Core.Grpc;
using Overt.Core.Grpc.Intercept;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overt.GrpcExample.Client.Tracer
{
    public class ConsoleTracer : IClientTracer
    {
        public IEnumerable<CallInvoker> CallInvokers { get; set; }

        public void Exception<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, Exception exception, TRequest request = null)
            where TRequest : class
            where TResponse : class
        {
            Console.WriteLine("some exception");
        }

        public void Finish<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            Console.WriteLine("finished request");
        }

        public void Request<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            // 修改Channel
            // context.Options.Headers.Add(Constants.MetadataKey_ChannelTarget, "127.0.0.1:10004");

            Console.WriteLine("start request");
        }

        public void Response<TRequest, TResponse>(TResponse response, ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            Console.WriteLine("end response");
        }
    }
}
