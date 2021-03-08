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
        private readonly IEndpointStrategy _strategy;
        private readonly GrpcClientOptions _options;
        public ClientCallInvoker(IEndpointStrategy strategy, GrpcClientOptions options)
        {
            if (strategy == null || options == null)
                throw new ArgumentNullException($"参数strategy/options不能为空");

            _strategy = strategy;
            _options = options;
            _maxRetry = _options.MaxRetry;
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
                    if (_options?.GetInvoker != null)
                    {
                        var invokers = _strategy.GetCallInvokers(_options.ServiceName);
                        callInvoker = _options.GetInvoker(invokers);
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