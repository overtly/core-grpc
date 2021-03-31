
namespace Overt.Core.Grpc.H2
{
    /// <summary>
    ///   <consulSettings>
    ///     <service address=""></service>
    ///   </consulSettings>
    /// </summary>
    public class ConsulServerSection
    {
        /// <summary>
        /// grpc配置
        /// </summary>
        public ConsulServiceElement Service
        {
            get; set;
        }
    }
}
