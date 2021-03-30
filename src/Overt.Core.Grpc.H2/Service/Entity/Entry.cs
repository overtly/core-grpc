using System;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 注册成功后对象
    /// </summary>
    public sealed class Entry : IDisposable
    {
        private readonly ConsulRegister _serverRegister;

        public Entry(ConsulRegister serverRegister, string serviceId)
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
