using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace Overt.Core.Grpc.Intercept
{
    internal class InterceptedServerHandler<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        readonly IServerTracer _tracer;
        readonly ServerCallContext _context;
        public InterceptedServerHandler(IServerTracer tracer, ServerCallContext context)
        {
            _tracer = tracer;
            _context = context;
        }

        public async Task<TResponse> UnaryServerHandler(TRequest request, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                _tracer.Request(request, _context);
                var response = await continuation(request, _context).ConfigureAwait(false);
                _tracer.Response(response, _context);
                _tracer.Finish(_context);
                return response;
            }
            catch (Exception ex)
            {
                _tracer.Exception(_context, ex, request);
                throw;
            }
        }

        public async Task<TResponse> ClientStreamingServerHandler(IAsyncStreamReader<TRequest> requestStream, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                var tracingRequestStream = new TracingAsyncServerStreamReader<TRequest>(requestStream, _context, _tracer.Request);
                var response = await continuation(tracingRequestStream, _context).ConfigureAwait(false);
                _tracer.Response(response, _context);
                _tracer.Finish(_context);
                return response;
            }
            catch (Exception ex)
            {
                _tracer.Exception<object>(_context, ex);
                throw;
            }
        }

        public async Task ServerStreamingServerHandler(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                var tracingResponseStream = new TracingServerStreamWriter<TResponse>(responseStream, _context, _tracer.Response);
                _tracer.Request(request, _context);
                await continuation(request, tracingResponseStream, _context).ConfigureAwait(false);
                _tracer.Finish(_context);
            }
            catch (Exception ex)
            {
                _tracer.Exception(_context, ex, request);
                throw;
            }
        }

        public async Task DuplexStreamingServerHandler(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                var tracingRequestStream = new TracingAsyncServerStreamReader<TRequest>(requestStream, _context, _tracer.Request);
                var tracingResponseStream = new TracingServerStreamWriter<TResponse>(responseStream, _context, _tracer.Response);
                await continuation(tracingRequestStream, tracingResponseStream, _context).ConfigureAwait(false);
                _tracer.Finish(_context);
            }
            catch (Exception ex)
            {
                _tracer.Exception<object>(_context, ex);
                throw;
            }
        }
    }
}