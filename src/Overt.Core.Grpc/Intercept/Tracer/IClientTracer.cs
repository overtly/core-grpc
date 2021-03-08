using Grpc.Core.Interceptors;
using System;

namespace Overt.Core.Grpc.Intercept
{
    public interface IClientTracer
    {
        void Request<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class;

        void Response<TRequest, TResponse>(TResponse response, ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class;

        void Finish<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class;

        void Exception<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, Exception exception, TRequest request = null)
            where TRequest : class
            where TResponse : class;
    }

}
