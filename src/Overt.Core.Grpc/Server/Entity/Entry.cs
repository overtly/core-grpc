using System;

namespace Overt.Core.Grpc
{
    /// <summary>
    /// 注册成功后对象
    /// </summary>
    public sealed class Entry : IDisposable
    {
        private readonly ServerRegister _serverRegister;

        public Entry(ServerRegister serverRegister, string serviceId)
        {
            ServiceId = serviceId;
            _serverRegister = serverRegister;
        }

        public string ServiceId { get; }

        public void Dispose()
        {
            _serverRegister.Deregister(ServiceId);
        }
    }
}
