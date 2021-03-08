using Consul;
using System;
using System.Linq;
using System.Net;
using System.Timers;

namespace Overt.Core.Grpc
{
    /// <summary>
    /// 服务注册
    /// </summary>
    public class ServerRegister
    {
        private readonly ConsulClient _client;
        private readonly object _locker = new object();
        private readonly Timer _selfCheckTimer;
        private readonly Func<string, DnsEndPoint, string> _genServiceId;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="address"></param>
        public ServerRegister(string address, Func<string, DnsEndPoint, string> genServiceId = null)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException($"consul address");

            _genServiceId = genServiceId ?? GenServiceId;
            _client = new ConsulClient((cfg) =>
            {
                var uriBuilder = new UriBuilder(address);
                cfg.Address = uriBuilder.Uri;
            });

            _selfCheckTimer = new Timer(ConsulTimespan.SelfCheckInterval.TotalSeconds * 1000);
        }

        #region Public Method
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="serviceElement">节点</param>
        /// <param name="genServiceId">节点的ServiceId</param>
        /// <param name="registered">注册完回调</param>
        /// <returns></returns>
        public void Register(Service.ServiceElement serviceElement, Action<Entry> registered)
        {
            #region RegisterService
            if (_client == null)
                throw new ArgumentNullException($"consul client");

            var serviceName = serviceElement.Name;
            var dnsEndPoint = GenServiceAddress(serviceElement);
            var registerResult = RegisterService(serviceName, dnsEndPoint, registered);
            if (!registerResult)
                throw new Exception($"overt: failed to register service {serviceName} on host:port {dnsEndPoint}");
            #endregion

            #region InitIntervalReport
            InitIntervalSelfCheckTimer(serviceName, dnsEndPoint);
            #endregion
        }

        /// <summary>
        /// 移除服务
        /// </summary>
        /// <param name="serviceId"></param>
        public void Deregister(string serviceId)
        {
            _client?.Agent?.ServiceDeregister(serviceId).GetAwaiter().GetResult();
        }
        #endregion

        #region Private Method
        /// <summary>
        /// 获取EndPoint
        /// </summary>
        /// <param name="serviceAddress"></param>
        /// <returns></returns>
        private DnsEndPoint GenServiceAddress(Service.ServiceElement service)
        {
            #region 获取ServiceAddress
            // 环境变量优先
            var serviceAddress = string.Empty;
            if (!string.IsNullOrEmpty(service.HostEnv))
            {
                var envVariableTarget = (EnvironmentVariableTarget[])Enum.GetValues(typeof(EnvironmentVariableTarget));
                foreach (EnvironmentVariableTarget item in envVariableTarget)
                {
                    serviceAddress = Environment.GetEnvironmentVariable(service.HostEnv, item);
                    if (!string.IsNullOrEmpty(serviceAddress))
                        break;
                }
            }
            // 读取本地
            if (string.IsNullOrEmpty(serviceAddress))
                serviceAddress = service.Host;
            //读取内网地址
            if (string.IsNullOrEmpty(serviceAddress))
                serviceAddress = IPHelper.GetLocalIntranetIP().ToString();
            #endregion

            var serviceHost = string.Empty;
            var servicePort = 0;
            var hostAndPort = serviceAddress.Split(':');

            if (hostAndPort.Length == 1)
                serviceHost = hostAndPort[0];
            else
            {
                serviceHost = hostAndPort[0];
                int.TryParse(hostAndPort[1], out servicePort);
            }
            if (servicePort <= 0)
                servicePort = service.Port;

            return new DnsEndPoint(serviceHost, servicePort);
        }

        /// <summary>
        /// Generate ServiceId
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="dnsEndPoint"></param>
        /// <returns></returns>
        private string GenServiceId(string serviceName, DnsEndPoint dnsEndPoint)
        {
            return $"{serviceName}-{dnsEndPoint.Host}:{dnsEndPoint.Port}";
        }

        /// <summary>
        /// Generate CheckId
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="dnsEndPoint"></param>
        /// <returns></returns>
        private string GenCheckId(string serviceName, DnsEndPoint dnsEndPoint)
        {
            return $"Check:{serviceName}-{dnsEndPoint.Host}:{dnsEndPoint.Port}";
        }

        /// <summary>
        /// Generate CheckName
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="dnsEndPoint"></param>
        /// <returns></returns>
        private string GenCheckName(string serviceName, DnsEndPoint dnsEndPoint)
        {
            return $"Check:{serviceName}-{dnsEndPoint.Host}:{dnsEndPoint.Port}";
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="dnsEndPoint"></param>
        /// <param name="genServiceId"></param>
        /// <param name="registered">注册成功后执行</param>
        /// <returns></returns>
        private bool RegisterService(string serviceName, DnsEndPoint dnsEndPoint, Action<Entry> registered = null)
        {
            var serviceId = _genServiceId(serviceName, dnsEndPoint);
            var checkId = GenCheckId(serviceName, dnsEndPoint);
            var checkName = GenCheckName(serviceName, dnsEndPoint);
            var acr = new AgentCheckRegistration
            {
                //ID = checkId,
                Name = checkName,
                TCP = $"{dnsEndPoint.Host}:{dnsEndPoint.Port}",
                Interval = ConsulTimespan.CheckInterval,
                Status = HealthStatus.Passing,
                DeregisterCriticalServiceAfter = ConsulTimespan.CriticalInterval,
                ServiceID = serviceId,
            };
            var asr = new AgentServiceRegistration
            {
                ID = serviceId,
                Name = serviceName,
                Address = dnsEndPoint.Host,
                Port = dnsEndPoint.Port,
                Check = acr
            };

            var res = _client.Agent.ServiceRegister(asr).Result;
            if (res.StatusCode != HttpStatusCode.OK)
                return false;

            registered?.Invoke(new Entry(this, serviceId));
            return true;
        }

        /// <summary>
        /// 初始化Timer
        /// </summary>
        private void InitIntervalSelfCheckTimer(string serviceName, DnsEndPoint dnsEndPoint)
        {
            _selfCheckTimer.Elapsed += (sender, e) =>
           {
               lock (_locker)
               {
                   _selfCheckTimer.Stop();
                   DoSelfCheck(serviceName, dnsEndPoint);
                   _selfCheckTimer.Start();
               }
           };
            _selfCheckTimer.Start();
        }

        /// <summary>
        /// 做反向检查
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="dnsEndPoint"></param>
        private void DoSelfCheck(string serviceName, DnsEndPoint dnsEndPoint)
        {
            if (_client == null)
                return;

            try
            {
                var response = _client.Health.Service(serviceName, "", true).Result;
                var servcieId = _genServiceId(serviceName, dnsEndPoint);
                var serviceEntry = response?.Response?.FirstOrDefault(oo => oo?.Service?.ID == servcieId);
                if (serviceEntry == null)
                {
                    RegisterService(serviceName, dnsEndPoint);
                }
            }
            catch
            {
                //异常忽略
            }
        }
        #endregion
    }
}
