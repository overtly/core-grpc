namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// IP地址
    /// </summary>
    public class GrpcEndpointElement
    {
        /// <summary>
        /// 服务IP
        /// </summary>
        public string Host
        {
            get; set;
        }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port
        {
            get; set;
        }

        /// <summary>
        /// to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(this.Host, ":", this.Port.ToString());
        }
    }
}
