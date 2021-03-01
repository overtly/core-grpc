using Grpc.Core;
using Grpc.Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Overt.Core.Grpc
{
    public class ServerCallInvoker : CallInvoker
    {
        public readonly Channel CurrentChannel;
        public ServerCallInvoker(Channel channel)
        {
            CurrentChannel = GrpcPreconditions.CheckNotNull(channel);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Calls.BlockingUnaryCall(CreateCall(method, host, options), request);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Calls.AsyncUnaryCall(CreateCall(method, host, options), request);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Calls.AsyncServerStreamingCall(CreateCall(method, host, options), request);
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return Calls.AsyncClientStreamingCall(CreateCall(method, host, options));
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return Calls.AsyncDuplexStreamingCall(CreateCall(method, host, options));
        }

        protected virtual CallInvocationDetails<TRequest, TResponse> CreateCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
            where TRequest : class
            where TResponse : class
        {
            var channel = CurrentChannel;
            if (options.Headers?.Any(oo => oo.Key == Constants.MetadataKey_Target) ?? false)
            {
                var entry = options.Headers.First(oo => oo.Key == Constants.MetadataKey_Target);
                channel = new Channel(entry.Value, ChannelCredentials.Insecure, Constants.DefaultChannelOptions);
            }
            return new CallInvocationDetails<TRequest, TResponse>(channel, method, host, options);
        }
    }
}