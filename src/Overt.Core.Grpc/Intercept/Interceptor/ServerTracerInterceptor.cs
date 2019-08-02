using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Threading.Tasks;

namespace Overt.Core.Grpc.Intercept
{
    /// <summary>
    /// 服务端拦截器
    /// </summary>
    internal class ServerTracerInterceptor : Interceptor
    {
        readonly IServerTracer _tracer;
        public ServerTracerInterceptor(IServerTracer tracer)
        {
            _tracer = tracer;
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return new InterceptedServerHandler<TRequest, TResponse>(_tracer, context)
                .ClientStreamingServerHandler(requestStream, continuation);
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return new InterceptedServerHandler<TRequest, TResponse>(_tracer, context)
                .DuplexStreamingServerHandler(requestStream, responseStream, continuation);
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return new InterceptedServerHandler<TRequest, TResponse>(_tracer, context)
                .ServerStreamingServerHandler(request, responseStream, continuation);
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            return new InterceptedServerHandler<TRequest, TResponse>(_tracer, context)
                .UnaryServerHandler(request, continuation);
        }
    }
}
