using System;
using System.Net;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// Grpc配置信息
    /// </summary>
    public class GrpcOptions
    {
        /// <summary>
        /// 监听地址
        /// </summary>
        internal string ListenAddress { get; set; }

        /// <summary>
        /// 配置文件地址 
        /// default: appsettings.json
        /// </summary>
        public string ConfigPath { get; set; }

        /// <summary>
        /// ServiceId生成
        /// </summary>
        public Func<string, DnsEndPoint, string> GenServiceId { get; set; }
    }
}
