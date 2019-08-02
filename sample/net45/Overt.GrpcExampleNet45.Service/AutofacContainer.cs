using Autofac;
using Overt.Core.Grpc.Intercept;
using Overt.GrpcExampleNet45.Service.Tracer;

namespace Overt.GrpcExampleNet45.Service
{
    public class AutofacContainer
    {
        public static IContainer Register()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build(Autofac.Builder.ContainerBuildOptions.None);
            return container;
        }
    }
}
