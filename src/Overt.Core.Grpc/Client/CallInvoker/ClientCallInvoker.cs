using Grpc.Core;
using Grpc.Core.Interceptors;
using Overt.Core.Grpc.Intercept;
using System;
using System.Collections.Generic;

namespace Overt.Core.Grpc
{
    internal sealed class ClientCallInvoker : CallInvoker
    {
        private readonly int _maxRetry;
        private readonly string _serviceName;
        private readonly IEndpointStrategy _strategy;
        private readonly IClientTracer _tracer;
        private List<Interceptor> _interceptors;
        public ClientCallInvoker(IEndpointStrategy strategy, string serviceName, int maxRetry = 0, IClientTracer tracer = null, List<Interceptor> interceptors = null)
        {
            _strategy = strategy;
            _serviceName = serviceName;
            _maxRetry = maxRetry;
            _tracer = tracer;
            _interceptors = interceptors;

            if (_tracer != null)
            {
                _interceptors = _interceptors ?? new List<Interceptor>();
                _interceptors.Add(new ClientTracerInterceptor(_tracer));
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


        #region Private Method
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
                    callInvoker = _strategy.GetCallInvoker(_serviceName);
                    if (callInvoker == null)
                    {
                        throw new ArgumentNullException($"{_serviceName}无可用节点");
                    }

                    var channel = callInvoker.Channel;
                    if (channel == null || channel.State == ChannelState.TransientFailure)
                    {
                        throw new RpcException(new Status(StatusCode.Unavailable, $"Channel Failure"));
                    }

                    if (_tracer != null)
                    {
                        _tracer.CallInvokers = _strategy.GetCallInvokers(_serviceName);
                    }
                    if (_interceptors?.Count > 0)
                    {
                        return call(callInvoker.Intercept(_interceptors.ToArray()));
                    }

                    return call(callInvoker);
                }
                catch (RpcException ex)
                {
                    // 服务不可用，拉入黑名单
                    if (ex.Status.StatusCode == StatusCode.Unavailable)
                        _strategy.Revoke(_serviceName, callInvoker);

                    if (0 > --retryLeft)
                    {
                        throw new Exception($"status: {ex.StatusCode}, node: {callInvoker?.Channel?.Target}, message: {ex.Message}", ex);
                    }
                }
            }
        }
        #endregion
    }
}