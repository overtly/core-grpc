using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;

namespace Overt.Core.Grpc
{
    internal sealed class ClientCallInvoker : CallInvoker
    {
        private readonly IEndpointStrategy _strategy;
        private readonly int _maxRetry;
        private readonly string _serviceName;
        private readonly List<Interceptor> _interceptors;
        private readonly Func<List<ServerCallInvoker>, ServerCallInvoker> _getInvoker;
        public ClientCallInvoker(
            IEndpointStrategy strategy,
            string serviceName, 
            int maxRetry, 
            List<Interceptor> interceptors = default, 
            Func<List<ServerCallInvoker>, ServerCallInvoker> getInvoker = default)
        {
            _strategy = strategy;
            _serviceName = serviceName;
            _maxRetry = maxRetry;
            _interceptors = interceptors;
            _getInvoker = getInvoker;
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
                    if (_getInvoker != null)
                    {
                        var invokers = _strategy.GetCallInvokers(_serviceName);
                        callInvoker = _getInvoker(invokers);
                    }
                    else
                        callInvoker = _strategy.GetCallInvoker(_serviceName);


                    if (callInvoker == null)
                    {
                        throw new ArgumentNullException($"{_serviceName}无可用节点");
                    }
                    if (callInvoker.Channel == null || callInvoker.Channel.State == ChannelState.TransientFailure)
                    {
                        throw new RpcException(new Status(StatusCode.Unavailable, $"Channel Failure"));
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