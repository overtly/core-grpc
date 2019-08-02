using Overt.Core.Grpc.Client;
#if !ASP_NET_CORE
using System.Configuration;
#endif

namespace Overt.Core.Grpc
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
    public class GrpcClientSection
#if !ASP_NET_CORE
        : ConfigurationSection
#endif
    {        /// <summary>
             /// grpc配置
             /// </summary>
#if !ASP_NET_CORE
        [ConfigurationProperty("service", IsRequired = true)]
#endif
        public GrpcServiceElement Service
        {
#if !ASP_NET_CORE
            get { return this["service"] as GrpcServiceElement; }
#else
        get; set;
#endif
        }
    }
}
