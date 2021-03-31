using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;

namespace Overt.Core.Grpc
{
    internal sealed class ClientCallInvoker : CallInvoker
    {
        private readonly GrpcClientOptions _options;
        private readonly IEndpointStrategy _strategy;
        private readonly Func<List<ServerCallInvoker>, ServerCallInvoker> _callInvokers;
        public ClientCallInvoker(
            GrpcClientOptions options,
            IEndpointStrategy strategy,
            Func<List<ServerCallInvoker>, ServerCallInvoker> callInvokers = default)
        {
            _options = options;
            _strategy = strategy;

            _callInvokers = callInvokers;
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Call(ci => ci.BlockingUnaryCall(method, host, options, request), _options.MaxRetry);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Call(ci => ci.AsyncUnaryCall(method, host, options, request), _options.MaxRetry);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Call(ci => ci.AsyncServerStreamingCall(method, host, options, request), _options.MaxRetry);
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return Call(ci => ci.AsyncClientStreamingCall(method, host, options), _options.MaxRetry);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return Call(ci => ci.AsyncDuplexStreamingCall(method, host, options), _options.MaxRetry);
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
                    if (_callInvokers != null)
                    {
                        var invokers = _strategy.GetCallInvokers(_options.ServiceName);
                        callInvoker = _callInvokers(invokers);
                    }
                    else
                        callInvoker = _strategy.GetCallInvoker(_options.ServiceName);


                    if (callInvoker == null)
                    {
                        throw new ArgumentNullException($"{_options.ServiceName}无可用节点");
                    }
                    if (callInvoker.Channel == null || callInvoker.Channel.State == ChannelState.TransientFailure)
                    {
                        throw new RpcException(new Status(StatusCode.Unavailable, $"Channel Failure"));
                    }

                    if (_options.Interceptors?.Count > 0)
                    {
                        return call(callInvoker.Intercept(_options.Interceptors.ToArray()));
                    }
                    return call(callInvoker);
                }
                catch (RpcException ex)
                {
                    // 服务不可用，拉入黑名单
                    if (ex.Status.StatusCode == StatusCode.Unavailable)
                        _strategy.Revoke(_options.ServiceName, callInvoker);

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