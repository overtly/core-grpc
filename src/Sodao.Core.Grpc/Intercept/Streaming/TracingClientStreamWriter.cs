using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Threading.Tasks;

namespace Sodao.Core.Grpc.Intercept
{
    internal class TracingClientStreamWriter<T, TResponse> : IClientStreamWriter<T>
        where T : class
        where TResponse : class
    {
        private readonly IClientStreamWriter<T> _writer;
        private readonly ClientInterceptorContext<T, TResponse> _context;
        private readonly Action<T, ClientInterceptorContext<T, TResponse>> _onWrite;
        private readonly Action _onComplete;

        public TracingClientStreamWriter(IClientStreamWriter<T> writer, ClientInterceptorContext<T, TResponse> context, Action<T, ClientInterceptorContext<T, TResponse>> onWrite, Action onComplete = null)
        {
            _writer = writer;
            _context = context;
            _onWrite = onWrite;
            _onComplete = onComplete;
        }

        public WriteOptions WriteOptions
        {
            get => _writer.WriteOptions;
            set => _writer.WriteOptions = value;
        }

        public Task WriteAsync(T message)
        {
            _onWrite(message, _context);
            return _writer.WriteAsync(message);
        }

        public Task CompleteAsync()
        {
            _onComplete?.Invoke();
            return _writer.CompleteAsync();
        }
    }
}