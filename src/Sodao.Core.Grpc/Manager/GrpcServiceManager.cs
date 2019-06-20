using Grpc.Core;
using Grpc.Core.Interceptors;
using Sodao.Core.Grpc.Intercept;
using System;
using System.Collections.Generic;

namespace Sodao.Core.Grpc
{
    /// <summary>
    /// Grpc服务管理
    /// </summary>
    public class GrpcServiceManager
    {
        static Server server;
        static ServerRegister serverRegister;
        static Entry discoveryEntry;

        #region Public Method
        /// <summary>
        /// Grpc服务启动
        /// </summary>
        /// <param name="service">grpc service definition</param>
        /// <param name="tracer">拦截器记录</param>
        /// <param name="interceptors">其他拦截器</param>
        /// <param name="configPath">配置文件路径 default: dllconfig/{namespace}.dll.[config/json]</param>
        /// <param name="channelOptions">Channel配置</param>
        /// <param name="whenException">==null => throw</param>
        public static void Start(
            ServerServiceDefinition service,
            IServerTracer tracer = null,
            List<Interceptor> interceptors = null,
            string configPath = "",
            List<ChannelOption> channelOptions = null,
            Action<Exception> whenException = null)
        {
            try
            {
                #region 启动服务
                var serviceElement = ResolveServiceConfiguration(configPath);
                if (tracer != null)
                {
                    tracer.ServiceName = serviceElement.Name;
                    interceptors = interceptors ?? new List<Interceptor>();
                    interceptors.Add(new ServerTracerInterceptor(tracer));
                }
                if (interceptors?.Count > 0)
                    service = service.Intercept(interceptors.ToArray());
                server = new Server(channelOptions)
                {
                    Services = { service },
                    Ports = { new ServerPort("0.0.0.0", serviceElement.Port, ServerCredentials.Insecure) }
                };
                server.Start();
                #endregion

                #region 注册服务
                var address = ResolveConsulConfiguration(serviceElement);
                if (string.IsNullOrEmpty(address))
                    return;
                serverRegister = new ServerRegister(address);
                serverRegister.Register(serviceElement, entry => discoveryEntry = entry);
                #endregion
            }
            catch (Exception ex)
            {
                Stop();
                InvokeException(ex, whenException);
            }
        }

        /// <summary>
        /// Grpc服务停止
        /// </summary>
        /// <param name="whenException">==null => throw</param>
        public static void Stop(Action<Exception> whenException = null)
        {
            try
            {
                serverRegister?.Deregister(discoveryEntry?.ServiceId);
                server?.ShutdownAsync().Wait();
            }
            catch (Exception ex)
            {
                InvokeException(ex, whenException);
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="configPath"></param>
        private static Service.ServiceElement ResolveServiceConfiguration(string configPath = "")
        {
            var sectionName = Constants.GrpcServerSectionName;
            var grpcSection = ConfigHelper.Get<GrpcServerSection>(sectionName, configPath);
            if (grpcSection == null)
                throw new ArgumentNullException(sectionName);

            var service = grpcSection.Service;
            if (service == null)
                throw new ArgumentNullException($"service");

            if (string.IsNullOrEmpty(service.Name))
                throw new ArgumentNullException("serviceName");

            if (service.Port <= 0)
                throw new ArgumentNullException("servicePort");

            return grpcSection.Service;
        }

        /// <summary>
        /// 解析Consul配置
        /// </summary>
        /// <returns></returns>
        private static string ResolveConsulConfiguration(Service.ServiceElement service)
        {
            var configPath = string.Empty;
#if !ASP_NET_CORE
            configPath = service.Registry?.Consul?.Path;
#else
            configPath = service.Consul?.Path;
#endif
            if (string.IsNullOrEmpty(configPath))
                return string.Empty;

            var consulSection = ConfigHelper.Get<ConsulServerSection>(Constants.ConsulServerSectionName, configPath);
            return consulSection?.Service?.Address;
        }

        /// <summary>
        /// 执行异常
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="whenException"></param>
        private static void InvokeException(Exception exception, Action<Exception> whenException = null)
        {
            if (whenException != null)
                whenException.Invoke(exception);
            else
                throw exception;
        }
        #endregion
    }
}
