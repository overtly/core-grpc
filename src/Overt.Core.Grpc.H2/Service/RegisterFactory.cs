using System;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// Grpc服务管理
    /// </summary>
    public class RegisterFactory
    {
        static Entry discoveryEntry;
        static ConsulRegister serverRegister;

        #region Public Method
        /// <summary>
        /// Consul注册
        /// </summary>
        /// <param name="servicgrpcOptionses">配置</param>
        /// <param name="whenException">==null => throw</param>
        public static void WithConsul(
            GrpcOptions grpcOptions = null,
            Action<Exception> whenException = null)
        {
            try
            {
                var serviceElement = ResolveServiceConfiguration(grpcOptions);

                #region 注册服务
                var address = ResolveConsulConfiguration(serviceElement);
                if (string.IsNullOrWhiteSpace(address))
                    return;
                serverRegister = new ConsulRegister(address, grpcOptions.GenServiceId);
                serverRegister.Register(serviceElement, entry => discoveryEntry = entry);
                #endregion
            }
            catch (Exception ex)
            {
                RemoveConsul();
                InvokeException(ex, whenException);
            }
        }

        /// <summary>
        /// 从consul中移除
        /// </summary>
        /// <param name="whenException">==null => throw</param>
        public static void RemoveConsul(Action<Exception> whenException = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(discoveryEntry?.ServiceId))
                    return;

                serverRegister?.Deregister(discoveryEntry?.ServiceId);
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
        private static ServiceElement ResolveServiceConfiguration(GrpcOptions grpcOptions = null)
        {
            var sectionName = Constants.GrpcServerSectionName;
            var grpcSection = ConfigBuilder.Build<GrpcServerSection>(sectionName, grpcOptions?.ConfigPath);
            if (grpcSection == null)
                throw new ArgumentNullException(sectionName);

            var service = grpcSection.Service;
            if (service == null)
                throw new ArgumentNullException($"service");

            if (string.IsNullOrWhiteSpace(service.Name))
                throw new ArgumentNullException("serviceName");

            if (service.Port <= 0)
            {
                if (!string.IsNullOrWhiteSpace(grpcOptions.ListenAddress))
                {
                    var splits = grpcOptions.ListenAddress?.Split(":");
                    if (splits?.Length == 3 && int.TryParse(splits[2], out int port))
                        service.Port = port;
                }
                if (service.Port <= 0)
                    throw new ArgumentNullException("servicePort");
            }

            return grpcSection.Service;
        }

        /// <summary>
        /// 解析Consul配置
        /// </summary>
        /// <returns></returns>
        private static string ResolveConsulConfiguration(ServiceElement service)
        {
            var configPath = service.Consul?.Path;
            if (string.IsNullOrWhiteSpace(configPath))
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
