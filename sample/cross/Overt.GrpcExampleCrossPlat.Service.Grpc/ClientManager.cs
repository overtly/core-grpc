using Grpc.Core;
using Overt.Core.Grpc;
using Overt.Core.Grpc.Intercept;
using System;
using System.Collections.Concurrent;
using System.IO;
using __GrpcService = Overt.GrpcExample.Service.Grpc.GrpcExampleService;
namespace Overt.GrpcExample.Service.Grpc
{
#if NET45 || NET46 || NET47
    public class ClientManager
    {
        public static IClientTracer Tracer { get; set; }
        public static string ConfigPath { get; set; } = "dllconfigs/Overt.GrpcExample.Service.Grpc.dll.config";
        public static __GrpcService.GrpcExampleServiceClient Instance
        {
            get
            {
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigPath);
                return GrpcClientManager<__GrpcService.GrpcExampleServiceClient>.Get(configPath, Tracer);
            }
        }
        private static readonly ConcurrentDictionary<Type, string> configMap = new ConcurrentDictionary<Type, string>();
        public static void Configure<T>(string configPath) { configMap.AddOrUpdate(typeof(T), configPath, (t, s) => configPath); }
        public static string GetConfigure<T>() { if (configMap.TryGetValue(typeof(T), out string configPath)) return configPath; return ConfigPath; }
    }
    public class ClientManager<T> : ClientManager where T : ClientBase
    {
        public static new T Instance
        {
            get
            {
                var configPath = GetConfigure<T>();
                var abConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath);
                return GrpcClientManager<T>.Get(abConfigPath, Tracer);
            }
        }
    }
#endif
}