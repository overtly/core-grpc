using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace Overt.Core.Grpc.Intercept
{
    internal class TracingServerStreamWriter<T> : IServerStreamWriter<T>
    {
        private readonly IServerStreamWriter<T> _writer;
        private readonly ServerCallContext _context;
        private readonly Action<T, ServerCallContext> _onWrite;

        public TracingServerStreamWriter(IServerStreamWriter<T> writer, ServerCallContext context, Action<T, ServerCallContext> onWrite)
        {
            _writer = writer;
            _context = context;
            _onWrite = onWrite;
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
    }
}