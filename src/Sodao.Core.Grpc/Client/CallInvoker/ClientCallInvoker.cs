using Grpc.Core;
using Sodao.Core.Grpc.Intercept;
using System;

namespace Sodao.Core.Grpc
{
    internal sealed class ClientCallInvoker : CallInvoker
    {
        private readonly int _maxRetry;
        private readonly string _serviceName;
        private readonly IEndpointStrategy _strategy;
        private readonly IClientTracer _tracer;
        public ClientCallInvoker(IEndpointStrategy strategy, string serviceName, int maxRetry = 0, IClientTracer tracer = null)
        {
            _strategy = strategy;
            _serviceName = serviceName;
            _maxRetry = maxRetry;
            _tracer = tracer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="call"></param>
        /// <param name="retryLeft"></param>
        /// <returns></returns>
        private TResponse Call<TResponse>(Func<CallInvoker, TResponse> call, int retryLeft)
        {
            while (true)
            {
                var callInvoker = default(ServerCallInvoker);
                try
                {
                    callInvoker = _strategy.Get(_serviceName);
                    if (callInvoker == null)
                        throw new ArgumentNullException("无任何可用连接");

                    var channel = callInvoker.Channel;
                    if (channel == null || channel.State == ChannelState.TransientFailure)
                        throw new RpcException(new Status(StatusCode.Unavailable, "Channel Failure"));

                    var response = default(TResponse);
                    if (_tracer != null)
                        response = call(callInvoker.ClientIntercept(_tracer));
                    else
                        response = call(callInvoker);
                    return response;
                }
                catch (RpcException ex)
                {
                    if (ex.Status.StatusCode == StatusCode.Unavailable)
                        _strategy.Revoke(_serviceName, callInvoker);

                    if (0 > --retryLeft)
                        throw;
                }
            }
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Call(ci => ci.BlockingUnaryCall(method, host, options, request), _maxRetry);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Call(ci => ci.AsyncUnaryCall(method, host, options, request), _maxRetry);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Call(ci => ci.AsyncServerStreamingCall(method, host, options, request), _maxRetry);
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return Call(ci => ci.AsyncClientStreamingCall(method, host, options), _maxRetry);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return Call(ci => ci.AsyncDuplexStreamingCall(method, host, options), _maxRetry);
        }
    }
}