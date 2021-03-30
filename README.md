# Overt.Core.Grpc

### 项目层次说明

> Overt.Core.Grpc v1.0.5  
> 如有疑问可直接加QQ：2292709323，微信：yaofengv，联系

#### 1. 项目目录

```
|-Config                                        配置模型
|
|-Client                                        客户端功能实现，服务发现
|
|-dllconfigs                                    配置文件保存
|
|-Intercept                                     拦截器
|-|-IServerTracer                               服务端拦截器接口
|-|-IClientTracer                               客户端拦截器接口
|  
|-Manager                                       启动、客户端调用类
|-|-GrpcServiceManager.cs                       服务端启动类
|-|-GrpcClientManager.cs                        客户端获取Client类
|
|-Server                                        服务端
|
|-GrpcServiceCollectionExtensions.cs            netcore注入
```

#### 2. 版本及支持

> - Nuget版本：V 1.0.5

> - 框架支持： Framewok 4.5 - 4.7 / NetStandard 2.0



#### 3. 项目依赖

> - NetStandard 2.0



```csharp
Consul 0.7.2.6  
Google.Protobuf 3.8.0
Grpc 1.21.0
Microsoft.Extensions.Configuration.Json 2.0.0
Microsoft.Extensions.Options.ConfigurationExtensions 2.0.0
```

> - Framwork 4.5 - 4.7



```csharp
Consul 0.7.2.6  
Google.Protobuf 3.8.0
Grpc 1.21.0
```

### 使用

#### 1. Nuget包引用

```csharp
Install-Package Overt.Core.Grpc -Version 1.0.5
```

<a name="dhmwfy"></a>
#### 2. 配置信息

优先级：{第三方配置中心} > 环境变量 >  Host内部配置 > 自动取IP**内网**

##### （1）服务端配置信息

> - 支持默认配置文件appsettings.json [Consul节点可不要，如无则不是集群]


```json
{
  "GrpcServer": {
    "Service": {
      "Name": "OvertGrpcServiceApp",                    // 服务名称使用服务名称去除点：OvertGrpcServiceApp
      "Host": "service.g.lan",                          // 专用注册的域名 （可选）格式：ip[:port=default]
      "HostEnv": "serviceaddress",                      // 获取注册地址的环境变量名字（可选，优先）环境变量值格式：ip[:port=default]
      "Port": 10001,                                    // 端口自定义
      "Consul": {
        "Path": "dllconfigs/consulsettings.json"        // Consul路径
      }
    }
  }
}
```

```xml
// 添加section
<configSections>
  <section name="grpcServer" type="Overt.Core.Grpc.GrpcServerSection, Overt.Core.Grpc" />
</configSections>

// 添加节点
<grpcServer>
  <service name="OvertGrpcServiceApp" port="10005" host="专用注册的域名（可选）格式：ip[:port=default]" hostEnv="获取注册地址的环境变量名字（可选）环境变量值格式：ip[:port=default]">
    <registry>
      <consul path="dllconfigs/Consul.config" />
    </registry>
  </service>
</grpcServer>
```

##### （2）客户端配置信息

> - 命名：[命名空间].dll.json 文件夹(dllconfigs)

```json
{
  "GrpcClient": {
    "Service": {
      "Name": "grpcservice",                        // 服务名称与服务端保持一致
      "MaxRetry":  0,                               // 最大可重试次数，默认不重试
      "Discovery": {
        "EndPoints": [                              // 单点模式
          {
            "Host": "127.0.0.1",
            "Port": 10001
          }
        ],
        "Consul": {                                 // Consul集群,集群优先原则
          "Path": "dllconfigs/consulsettings.json"
        }
      }
    }
  }
}
```

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="grpcClient" type="Overt.Core.Grpc.GrpcClientSection, Overt.Core.Grpc"/>
  </configSections>

  <grpcClient>
    <service name="" maxRetry="0">
      <discovery>
        <server>
          <endpoint host="" port=""></endpoint>
          <endpoint host="" port=""></endpoint>
        </server>
        <consul path="dllconfigs/Consul.config"></consul>
      </discovery>
    </service>
  </grpcClient>
</configuration>
```

##### （3）Consul配置文件

> - 命名：consulsettings.json 不要改动

```json
{
  "ConsulServer": {
    "Service": {
      "Address": "http://consul.g.lan"     // 默认8500端口
    }
  }
}
```

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="consulServer" type="Overt.Core.Grpc.ConsulServerSection, Overt.Core.Grpc"/>
  </configSections>
  <consulServer>
    <service address="http://consul.g.lan"></service>
  </consulServer>
</configuration>
```

#### 3. 服务端的使用

#### （1）NetCore

> - 强制依赖注入模式

```csharp
services.AddSingleton<GrpcExampleService.GrpcExampleServiceBase, GrpcExampleServiceImpl>();          // Grpc服务的实现
services.AddSingleton<IHostedService, GrpcExampleHostedService>();                                   // Grpc服务启动服务类：如下
services.AddGrpcTracer<ConsoleTracer>();                                                             // Grpc注入拦截器，继承IServerTracer（可选）

// 使用第三方配置
services.AddGrpcConfig(config => 
{
    // 以配置中心apollo为例
    config.AddApollo(context.Configuration.GetSection("apollo")).AddDefault();
});
```

```csharp
using Microsoft.Extensions.Hosting;
using Overt.Core.Grpc;
using Overt.GrpcExample.Service.Grpc;
using System.Threading;
using System.Threading.Tasks;

namespace Overt.GrpcService.App
{
    public class GrpcService : IHostedService
    {
        GrpcExampleService.GrpcExampleServiceBase _grpcServiceBase;
        IServerTracer _tracer;
        public GrpcService(GrpcExampleService.GrpcExampleServiceBase serviceBase, IServerTracer tracer)         // 依赖注入Grpc服务基础类
        {
            _serviceBase = serviceBase;
            _tracer = tracer;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                GrpcServiceManager.Start(GrpcExampleService.BindService(_serviceBase), _tracer);
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                GrpcServiceManager.Stop();
            }, cancellationToken);
        }
    }
}
```

> - 实现类写法

```csharp
原因：服务启动的时候是一个单例，那么所有服务之下的全部是单实例，而数据层需要使用多实例

// 只注入 IServiceProvider
IServiceProvider _provider;
public GrpcExampleServiceImpl(IServiceProvider provider)
{
    _provider = provider;
}

// 其他利用provider即时获取
using(var scope = _provider.CreateSocpe())
{
    var _userService = scope.ServiceProvider.GetService<IUserService>();
}
```

#### （2）Framework 4.6

> - 直接调用GrpcServiceManager来启动



```csharp
using Grpc.Core;
using Overt.Core.Grpc;
using Overt.Log;
using System;

namespace Overt.GrpcService
{
    public class MainService
    {
        public MainService()
        {
            
        }
        public void Start(string serviceName)               // 启动服务
        {
            GrpcServiceManager.Start(Library.GrpcService.BindService(new GrpcServiceImpl()), tracer: new ConsoleTracer(), whenException: (ex) =>
            {
                LogHelper.Info("", ex);
            });
        }

        public void Stop(string serviceName)                // 停止服务
        {
            GrpcServiceManager.Stop();
        }
        public void ShutDown(string serviceName)
        {
            GrpcServiceManager.Stop();
        }
    }
}
```

#### 4. 客户端使用

#### （1）NetCore

> - 强制依赖注入模式

> - 配置文件默认使用    [命名空间].dll.json     可通过vs.menu工具生成nuget包

> - 注入中直接调用如下


```csharp
// 注入Grpc客户端
services.AddGrpcClient();

// 自定义配置文件 / 默认使用命名空间.dll.json
services.Configure<GrpcClientOptions<GrpcExampleServiceClient>>((cfg) =>
{
    cfg.ConfigPath = "dllconfig/Overt.GrpcExample.Service.Grpc.dll.json";  // 可不传递
});

// 使用第三方配置
services.AddGrpcConfig(config => 
{
    // 以配置中心apollo为例
    config.AddApollo(context.Configuration.GetSection("apollo")).AddDefault();
});


// 获取注入的对象
IGrpcClient<GrpcExampleServiceClient> _grpcClient;
public IndexModel(IGrpcClient<GrpcExampleServiceClient> grpcClient)
{
    _grpcClient = grpcClient;
}


var res = _grpcClient.Client.Ask(new Service.Grpc.AskRequest() { Key = "abc" });
```

#### （2）Framework

> - 客户端代理类，编译在Dll中，源码如下，可忽略

```csharp
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
        public static IClientTracer Tracer { get; set; } = default(IClientTracer);
        private static string DefaultConfigPath { get; set; } = "dllconfigs/Overt.GrpcExample.Service.Grpc.dll.config";
        public static __GrpcService.GrpcExampleServiceClient Instance
        {
            get
            {
                return ClientManager<__GrpcService.GrpcExampleServiceClient>.Instance;
            }
        }
        private static readonly ConcurrentDictionary<Type, string> configMap = new ConcurrentDictionary<Type, string>();
        public static void Configure<T>(string configPath) { configMap.AddOrUpdate(typeof(T), configPath, (t, s) => configPath); }
        public static string GetConfigure<T>() { if (configMap.TryGetValue(typeof(T), out string configPath)) return configPath; return DefaultConfigPath; }
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
```

> - 使用代理类执行


```
// 自定义配置文件 / 默认使用命名空间.dll.json / 在主进程入口进行配置T为服务Client
ClientManger.Configure<T>("dllconfig/abc.config");

ClientManager.Instance.[Method]

// T为服务Client
ClientManager<T>.Instance.[Method]
```

#### 5. 更新说明

- 2021-03-30 v 1.0.5
- 
> 1. Tracer提供修改Channel Target能力，可供外部自定义选择节点；
> 2. 升级底层驱动为最新版本
> 3. 增加自定义Interceptor的注入
> 4. 调整ServiceStart的参数配置，使用GrpcOptions进行承接
> 5. 增加invokers的自定义获取，增加ServiceId的获取
> 6. 支持framework版本的自定义invoker获取策略


- 2021-02-25 v 1.0.4.1
- 
> 1. 增加第三方配置的支持，比如apollo，可使用services.AddGrpcConfig进行扩展(目前仅支持dotnetcore)


- 2019-11-28 v 1.0.3.1
- 
> 1. 优化对Consul新版本的支持



- 2019-11-28 v 1.0.3
- 
> 1. 注册中心监听到变动后，忽略黑名单，本地连接全部重置



- 2019-09-29 v 1.0.2
> 1. 客户端使用Consul注册中心，支持单服务变动监听，新注册服务或者服务挂掉，更加实时



- 2019-08-16 v 1.0.1
- 
> 1. 支持多服务模式



- 2019-08-01 v 1.0.0
- 
> 1. 修改命名空间，更新nuget包为Overt.Core.Grpc 更新默认版本为1.0.0



- 2019-06-05 v 1.0.10.2
- 
> 1. 客户端优化连接服务失败的情况下，拉入黑名单，导致节点不存在的问题



- 2019-06-05 v 1.0.10
- 
> 1. 升级Grpc版本为 1.21.0
> 2. 升级Google.Protobuf版本为3.8.0



- 2019-03-06 v 1.0.9

> 1. 升级Grpc版本为 1.19.0
> 2. Consul节点默认Passing
> 3. ConsulSettings添加环境变量支持



- 2019-01-29 v 1.0.8

> 1. 升级Grpc版本为 1.18.0
> 2. 其他一系列升级以及Bug修复



- 2018-11-13 v 1.0.6.10 (****)

> 1. 添加IServerTracer作为服务端拦截器接口，实现即可拦截
> 2. 添加IClientTracer作为客户端拦截器接口，实现即可拦截
> 3. 升级Google.Protobuf v 3.6.1
> 4. **重要**[下载地址](http://10.0.60.89:8082/)

```csharp
Overt.Core.Grpc.dll
Consul.dll
Grpc.Core.dll
Google.Protobuf.dll
runtime/*
libgrpc_*
grpc_csharp_*
App.config/Web.config
```



- 2018-11-10 v 1.0.6.9

> 1. 升级Grpc v1.16.0 支持Intercept



- 2018-09-07 v 1.0.6.6

> 1. 影响服务端驱动
> 2. 更改ServciceHost获取以环境变量优先



- 2018-08-24 v 1.0.6.5

> 1. 影响客户端驱动
> 2. 修复发布过程中异常加入黑名单后，连接全部丢失问题



- 2018-08-21 v 1.0.6.4

> 1. 添加服务与Consul自检功能，升级Consul v0.7.2.6 / Grpc v1.14.1.0
> 2. 升级该版本需要同时发布**以下内容**

```csharp
Overt.Core.Grpc.dll
Consul.dll
Grpc.Core.dll
Google.Protobuf.dll
runtime/*
libgrpc_*
grpc_csharp_*
App.config/Web.config
```

---


