using Grpc.Core;
using Grpc.Core.Interceptors;
using System;

namespace Overt.Core.Grpc.Intercept
{
    internal class InterceptedClientHandler<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        readonly IClientTracer _tracer;
        readonly ClientInterceptorContext<TRequest, TResponse> _context;
        public InterceptedClientHandler(IClientTracer tracer, ClientInterceptorContext<TRequest, TResponse> context)
        {
            _tracer = tracer;
        }

        public TResponse BlockingUnaryCall(TRequest request, Interceptor.BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                _tracer.Request(request, _context);
                var response = continuation(request, _context);
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

        public AsyncUnaryCall<TResponse> AsyncUnaryCall(TRequest request, Interceptor.AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            _tracer.Request(request, _context);
            var rspCnt = continuation(request, _context);
            var rspAsync = rspCnt.ResponseAsync.ContinueWith(rspTask =>
            {
                try
                {
                    var response = rspTask.Result;
                    _tracer.Response(response, _context);
                    _tracer.Finish(_context);
                    return response;
                }
                catch (AggregateException ex)
                {
                    _tracer.Exception(_context, ex.InnerException, request);
                    throw ex.InnerException;
                }
            });
            return new AsyncUnaryCall<TResponse>(rspAsync, rspCnt.ResponseHeadersAsync, rspCnt.GetStatus, rspCnt.GetTrailers, rspCnt.Dispose);
        }

        public AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall(TRequest request, Interceptor.AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            _tracer.Request(request, _context);
            var rspCnt = continuation(request, _context);
            var tracingResponseStream = new TracingAsyncClientStreamReader<TResponse, TRequest>(rspCnt.ResponseStream, _context, _tracer.Response, _tracer.Finish, _tracer.Exception);
            return new AsyncServerStreamingCall<TResponse>(tracingResponseStream, rspCnt.ResponseHeadersAsync, rspCnt.GetStatus, rspCnt.GetTrailers, rspCnt.Dispose);
        }

        public AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall(Interceptor.AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var rspCnt = continuation(_context);
            var tracingRequestStream = new TracingClientStreamWriter<TRequest, TResponse>(rspCnt.RequestStream, _context, _tracer.Request);
            var rspAsync = rspCnt.ResponseAsync.ContinueWith(rspTask =>
            {
                try
                {
                    var response = rspTask.Result;
                    _tracer.Response(response, _context);
                    _tracer.Finish(_context);
                    return response;
                }
                catch (AggregateException ex)
                {
                    _tracer.Exception(_context, ex.InnerException, null);
                    throw ex.InnerException;
                }
            });
            return new AsyncClientStreamingCall<TRequest, TResponse>(tracingRequestStream, rspAsync, rspCnt.ResponseHeadersAsync, rspCnt.GetStatus, rspCnt.GetTrailers, rspCnt.Dispose);
        }

        public AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall(Interceptor.AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var rspCnt = continuation(_context);
            var tracingRequestStream = new TracingClientStreamWriter<TRequest, TResponse>(rspCnt.RequestStream, _context, _tracer.Request);
            var tracingResponseStream = new TracingAsyncClientStreamReader<TResponse, TRequest>(rspCnt.ResponseStream, _context, _tracer.Response, _tracer.Finish, _tracer.Exception);
            return new AsyncDuplexStreamingCall<TRequest, TResponse>(tracingRequestStream, tracingResponseStream, rspCnt.ResponseHeadersAsync, rspCnt.GetStatus, rspCnt.GetTrailers, rspCnt.Dispose);
        }
    }
}