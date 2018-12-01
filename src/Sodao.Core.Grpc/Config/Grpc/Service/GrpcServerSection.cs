using Sodao.Core.Grpc.Service;
#if NET45 || NET46 || NET47
using System.Configuration;
#endif

namespace Sodao.Core.Grpc
{
    /// <summary>
    ///   Grpc配置
    ///   <grpcServer>
    ///     <service name="" host="" port="">
    ///         <registry>
    ///             <consul path="" ></consul>
    ///         </registry>
    ///     </service>
    ///   </grpcServer>
    /// </summary>
    public class GrpcServerSection
#if NET45 || NET46 || NET47
        : ConfigurationSection
#endif
    {
#if NET45 || NET46 || NET47
        /// <summary>
        /// grpc配置
        /// </summary>
        [ConfigurationProperty("service", IsRequired = true)]
#endif
        public ServiceElement Service
        {
#if NET45 || NET46 || NET47
            get { return this["service"] as ServiceElement; }
#else
            get; set;
#endif

        }

    }
}
