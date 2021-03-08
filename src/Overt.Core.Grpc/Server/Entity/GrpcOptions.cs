using Grpc.Core;
using Grpc.Core.Interceptors;
using Overt.Core.Grpc.Intercept;
using System;
using System.Collections.Generic;
using System.Net;

namespace Overt.Core.Grpc
{
    /// <summary>
    /// Grpc配置信息
    /// </summary>
    public class GrpcOptions
    {
        /// <summary>
        /// 配置文件地址 
        /// default: appsettings.json
        /// </summary>
        public string ConfigPath { get; set; }

        /// <summary>
        /// 自定义拦截器
        /// default: null
        /// </summary>
        public List<Interceptor> Interceptors { get; set; }

        /// <summary>
        /// tracer拦截器
        /// default: null
        /// </summary>
        public IServerTracer Tracer { get; set; }

        /// <summary>
        /// Channel配置
        /// </summary>
        public List<ChannelOption> ChannelOptions { get; set; }

        /// <summary>
        /// ServiceId生成
        /// </summary>
        public Func<string, DnsEndPoint, string> GenServiceId { get; set; }
    }
}
