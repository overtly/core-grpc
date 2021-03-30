using Grpc.Core;
using Grpc.Core.Interceptors;
using Overt.Core.Grpc.Intercept;
using System;
using System.Collections.Generic;

namespace Overt.Core.Grpc
{
    /// <summary>
    /// Grpc服务管理
    /// </summary>
    public class GrpcServiceManager
    {
        static Server server;
        static Entry discoveryEntry;
        static ServerRegister serverRegister;

        #region Public Method
        /// <summary>
        /// Grpc服务启动
        /// </summary>
        /// <param name="service">grpc service definition</param>
        /// <param name="grpcOptionBuilder">配置信息</param>
        /// <param name="whenException">==null => throw</param>
        public static void Start(
            ServerServiceDefinition service,
            Action<GrpcOptions> grpcOptionBuilder = null,
            Action<Exception> whenException = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            var services = new List<ServerServiceDefinition>() { service };
            Start(services, grpcOptionBuilder, whenException);
        }

        /// <summary>
        /// Grpc服务启动，注册多个服务实现
        /// </summary>
        /// <param name="services">grpc service definition</param>
        /// <param name="whenException">==null => throw</param>
        public static void Start(
            IEnumerable<ServerServiceDefinition> services,
            Action<GrpcOptions> grpcOptionBuilder = null,
            Action<Exception> whenException = null)
        {
            try
            {
                var grpcOptions = new GrpcOptions();
                grpcOptionBuilder?.Invoke(grpcOptions);

                #region 启动服务
                var serviceElement = ResolveServiceConfiguration(grpcOptions);
                if (grpcOptions?.Tracer != null)
                {
                    grpcOptions.Tracer.ServiceName = serviceElement.Name;
                    grpcOptions.Interceptors.Add(new ServerTracerInterceptor(grpcOptions.Tracer));
                }
                server = new Server(grpcOptions.ChannelOptions)
                {
                    Ports = { new ServerPort("0.0.0.0", serviceElement.Port, ServerCredentials.Insecure) }
                };
                foreach (var service in services)
                {
                    if (grpcOptions.Interceptors?.Count > 0)
                        server.Services.Add(service.Intercept(grpcOptions.Interceptors.ToArray()));
                    else
                        server.Services.Add(service);
                }
                server.Start();
                #endregion

                #region 注册服务
                var address = ResolveConsulConfiguration(serviceElement);
                if (string.IsNullOrEmpty(address))
                    return;
                serverRegister = new ServerRegister(address, grpcOptions.GenServiceId);
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
        private static Service.ServiceElement ResolveServiceConfiguration(GrpcOptions grpcOptions = null)
        {
            var sectionName = Constants.GrpcServerSectionName;
            var grpcSection = ConfigBuilder.Build<GrpcServerSection>(sectionName, grpcOptions?.ConfigPath);
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

            var consulSection = ConfigBuilder.Build<ConsulServerSection>(Constants.ConsulServerSectionName, configPath);
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
