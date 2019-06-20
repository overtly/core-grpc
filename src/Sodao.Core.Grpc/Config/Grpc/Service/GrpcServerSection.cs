using Sodao.Core.Grpc.Service;
#if !ASP_NET_CORE
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
#if !ASP_NET_CORE
        : ConfigurationSection
#endif
    {
#if !ASP_NET_CORE
        /// <summary>
        /// grpc配置
        /// </summary>
        [ConfigurationProperty("service", IsRequired = true)]
#endif
        public ServiceElement Service
        {
#if !ASP_NET_CORE
            get { return this["service"] as ServiceElement; }
#else
            get; set;
#endif

        }

    }
}
