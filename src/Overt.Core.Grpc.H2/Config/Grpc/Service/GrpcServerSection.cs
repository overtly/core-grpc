namespace Overt.Core.Grpc.H2
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
    {
        /// <summary>
        /// 
        /// </summary>
        public ServiceElement Service { get; set; }
    }
}
