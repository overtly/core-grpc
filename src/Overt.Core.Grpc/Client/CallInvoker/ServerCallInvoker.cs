using Grpc.Core;
using Grpc.Core.Utils;

namespace Overt.Core.Grpc
{
    public class ServerCallInvoker : CallInvoker
    {
        public string ServiceId;
        public Channel Channel;
        public ServerCallInvoker(string serviceId, Channel channel)
        {
            ServiceId = serviceId;
            Channel = GrpcPreconditions.CheckNotNull(channel);
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
            //if (options.Headers?.Any(oo => oo.Key == Constants.MetadataKey_ChannelTarget) ?? false)
            //{
            //    var entry = options.Headers.First(oo => oo.Key == Constants.MetadataKey_ChannelTarget);
            //    Channel = new Channel(entry.Value, ChannelCredentials.Insecure, Constants.DefaultChannelOptions);
            //}
            return new CallInvocationDetails<TRequest, TResponse>(Channel, method, host, options);
        }
    }
}