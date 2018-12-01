using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Sodao.Core.Grpc
{
    /// <summary>
    /// 注册中心服务发现
    /// </summary>
    internal sealed class StickyEndpointDiscovery : IEndpointDiscovery
    {
        private readonly ConsulClient _client;
        public StickyEndpointDiscovery(string serviceName, string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("consul address");

            _client = new ConsulClient((cfg) =>
            {
                var uriBuilder = new UriBuilder(address);
                cfg.Address = uriBuilder.Uri;
            });

            ServiceName = serviceName;
        }

        public string ServiceName { get; set; }

        public List<string> FindServiceEndpoints(bool filterBlack = true)
        {
            if (_client == null)
                throw new ArgumentNullException("consul client");

            var targets = new List<string>();
            try
            {
                var r = _client.Health.Service(ServiceName, "", true).Result;
                if (r.StatusCode != HttpStatusCode.OK)
                    throw new ApplicationException($"failed to query consul server");

                targets = r.Response
                           .Select(x => $"{x.Service.Address}:{x.Service.Port}")
                           .Where(x => !ServiceBlackPlicy.In(x) || !filterBlack)
                           .ToList();

                //var res = _client.Catalog.Service(ServiceName).Result;
                //if (res.StatusCode != HttpStatusCode.OK)
                //    throw new ApplicationException($"Failed to query services");

                //targets = res.Response
                //             .Select(x => $"{x.ServiceAddress}:{x.ServicePort}")
                //             .Where(x => !ServiceBlackPlicy.In(x) || !filterBlack)
                //             .ToList();
                //return targets;
            }
            catch { }
            return targets;
        }
    }
}