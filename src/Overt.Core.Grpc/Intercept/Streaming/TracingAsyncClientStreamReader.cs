using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Overt.Core.Grpc.Intercept
{
    internal class TracingAsyncClientStreamReader<T, TRequest> : IAsyncStreamReader<T>
        where TRequest : class
        where T : class
    {
        private readonly IAsyncStreamReader<T> _reader;
        private readonly ClientInterceptorContext<TRequest, T> _context;
        private readonly Action<T, ClientInterceptorContext<TRequest, T>> _onMessage;
        private readonly Action<ClientInterceptorContext<TRequest, T>> _onStreamEnd;
        private readonly Action<ClientInterceptorContext<TRequest, T>, Exception, TRequest> _onException;

        public TracingAsyncClientStreamReader(IAsyncStreamReader<T> reader,
            ClientInterceptorContext<TRequest, T> context,
            Action<T, ClientInterceptorContext<TRequest, T>> onMessage,
            Action<ClientInterceptorContext<TRequest, T>> onStreamEnd = null,
            Action<ClientInterceptorContext<TRequest, T>, Exception, TRequest> onException = null)
        {
            _reader = reader;
            _context = context;
            _onMessage = onMessage;
            _onStreamEnd = onStreamEnd;
            _onException = onException;
        }

        public T Current => _reader.Current;

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            try
            {
                var hasNext = await _reader.MoveNext(cancellationToken).ConfigureAwait(false);
                if (hasNext)
                {
                    _onMessage?.Invoke(Current, _context);
                }
                else
                {
                    _onStreamEnd?.Invoke(_context);
                }

                return hasNext;
            }
            catch (Exception ex)
            {
                _onException?.Invoke(_context, ex, default(TRequest));
                throw;
            }
        }
    }
}