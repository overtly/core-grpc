using Grpc.Core;
using Grpc.Core.Interceptors;
using Overt.Core.Grpc;
using Overt.Core.Grpc.Intercept;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using __GrpcService = Overt.GrpcExample.Service.Grpc.GrpcExampleService;
namespace Overt.GrpcExample.Service.Grpc
{
#if NET45 || NET46 || NET47
public class ClientManager {
public static IClientTracer Tracer { get; set; } = default(IClientTracer);
public static List<Interceptor> Interceptors { get; } = new List<Interceptor>();
private static string DefaultConfigPath { get; set; } = "dllconfigs/Overt.GrpcExample.Service.Grpc.dll.config";
public static __GrpcService.GrpcExampleServiceClient Instance{get{
return ClientManager<__GrpcService.GrpcExampleServiceClient>.Instance;
} }
public static __GrpcService.GrpcExampleServiceClient CreateInstance(Func<List<ServerCallInvoker>, ServerCallInvoker> getInvoker = null){
return ClientManager<__GrpcService.GrpcExampleServiceClient>.CreateInstance(getInvoker);
}
private static readonly ConcurrentDictionary<Type, string> configMap = new ConcurrentDictionary<Type, string>();
public static void Configure<T>(string configPath) { configMap.AddOrUpdate(typeof(T), configPath, (t, s) => configPath); }
public static string GetConfigure<T>() { if (configMap.TryGetValue(typeof(T), out string configPath)) return configPath; return DefaultConfigPath;}
}
public class ClientManager<T> : ClientManager where T : ClientBase
{
public static new T Instance => CreateInstance();
public static new T CreateInstance(Func<List<ServerCallInvoker>, ServerCallInvoker> getInvoker = null) {
var configPath = GetConfigure<T>();
var options = new GrpcClientOptions() { Tracer = Tracer };
if (Interceptors?.Count > 0) options.Interceptors.AddRange(Interceptors);
return GrpcClientManager<T>.Get(configPath, options, getInvoker);
}
}
#endif
}