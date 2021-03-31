using Grpc.Core;
using Grpc.Core.Interceptors;
using Overt.Core.Grpc.Intercept;
using System.Collections.Generic;

namespace Overt.Core.Grpc
{
    /// <summary>
    /// 全局客户端配置模型
    /// </summary>
    public class GrpcClientOptions
    {
        /// <summary>
        /// Json文件
        /// defaultValue: dllconfigs/clientsettings.json 
        /// </summary>
        public string ConfigPath { get; set; }

        /// <summary>
        /// tracer拦截器
        /// </summary>
        public IClientTracer Tracer { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetry { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Interceptor"/> instances used to configure a gRPC client pipeline.
        /// </summary>
        public List<Interceptor> Interceptors { get; } = new List<Interceptor>();

        /// <summary>
        /// 配置ChannelOptions
        /// </summary>
        public List<ChannelOption> ChannelOptions { get; set; }
    }

    /// <summary>
    /// 单服务客户端配置
    /// </summary>
    public class GrpcClientOptions<T> : GrpcClientOptions where T : ClientBase
    {
        public GrpcClientOptions()
        {

        }

        public GrpcClientOptions(GrpcClientOptions options)
        {
            if (options == null)
                return;

            ConfigPath = options.ConfigPath;
            Tracer = options.Tracer;
            ServiceName = options.ServiceName;
            MaxRetry = options.MaxRetry;
            ChannelOptions = options.ChannelOptions;
            if (options.Interceptors?.Count > 0)
                Interceptors.AddRange(options.Interceptors);
        }
    }
}
