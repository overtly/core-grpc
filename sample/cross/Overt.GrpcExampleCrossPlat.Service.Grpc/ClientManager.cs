using Overt.Core.Grpc;
using Overt.Core.Grpc.Intercept;
using System;
using System.IO;
using __GrpcService = Overt.GrpcExample.Service.Grpc.GrpcExampleService;
namespace Overt.GrpcExample.Service.Grpc
{
#if NET45 || NET46 || NET47
public class ClientManager {
public static IClientTracer Tracer { get; set; } = default(IClientTracer);
public static __GrpcService.GrpcExampleServiceClient Instance{get{
var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dllconfigs/Overt.GrpcExample.Service.Grpc.dll.config");
return GrpcClientManager<__GrpcService.GrpcExampleServiceClient>.Get(configPath, Tracer);
} }
}
#endif
}