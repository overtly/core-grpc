using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overt.Core.Grpc
{
   public class GrpcClientFactoryOptions
    {
        /// <summary>
        /// Gets a list of <see cref="Interceptor"/> instances used to configure a gRPC client pipeline.
        /// </summary>
        public IList<Interceptor> Interceptors { get; } = new List<Interceptor>();
    }
}
