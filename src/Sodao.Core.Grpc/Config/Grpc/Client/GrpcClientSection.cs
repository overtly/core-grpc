using Sodao.Core.Grpc.Client;
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
    public class GrpcClientSection
#if NET45 || NET46 || NET47
        : ConfigurationSection
#endif
    {        /// <summary>
             /// grpc配置
             /// </summary>
#if NET45 || NET46 || NET47
        [ConfigurationProperty("service", IsRequired = true)]
#endif
        public GrpcServiceElement Service
        {
#if NET45 || NET46 || NET47
            get { return this["service"] as GrpcServiceElement; }
#else
        get; set;
#endif
        }
    }
}
