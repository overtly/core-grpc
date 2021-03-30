using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Overt.Core.Grpc.Intercept
{
    internal class TracingAsyncServerStreamReader<T> : IAsyncStreamReader<T>
    {
        private readonly IAsyncStreamReader<T> _reader;
        private readonly ServerCallContext _context;
        private readonly Action<T, ServerCallContext> _onMessage;
        private readonly Action _onStreamEnd;
        private readonly Action<Exception> _onException;

        public TracingAsyncServerStreamReader(IAsyncStreamReader<T> reader, ServerCallContext context, Action<T, ServerCallContext> onMessage, Action onStreamEnd = null, Action<Exception> onException = null)
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
                    _onStreamEnd?.Invoke();
                }

                return hasNext;
            }
            catch (Exception ex)
            {
                _onException?.Invoke(ex);
                throw;
            }
        }
    }
}