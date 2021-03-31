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
    public class GrpcClientSection
    {
        /// <summary>
        /// grpc配置
        /// </summary>
        public GrpcServiceElement Service { get; set; }
    }
}
