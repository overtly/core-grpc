### 项目层次说明

> Sodao.Core.Grpc v1.0.0

#### 1. 项目目录

```
|-Config                                        配置模型
|
|-Client                                        客户端功能实现，服务发现
|
|-dllconfigs                                    配置文件保存
|
|-Intercept                                     拦截器
|-|-IServerTracer                             服务端拦截器接口
|-|-IClientTracer                             客户端拦截器接口
|  
|-Manager                                       启动、客户端调用类
|-|-GrpcServiceManager.cs                     服务端启动类
|-|-GrpcClientManager.cs                      客户端获取Client类
|
|-Server                                        服务端
|
|-GrpcServiceCollectionExtensions.cs            netcore注入
```


#### 2. 版本及支持

> * Nuget版本：V 1.0.0
> * 框架支持： Framewok 4.5 - 4.7 / NetStandard 2.0


#### 3. 项目依赖

> * NetStandard 2.0

```
Consul 0.7.2.6  
Google.Protobuf 3.6.1
Grpc 1.16.0
Microsoft.Extensions.Configuration.Json 2.0.0
Microsoft.Extensions.Options.ConfigurationExtensions 2.0.0
```

> * Framwork 4.5 - 4.7

```
Consul 0.7.2.6  
Google.Protobuf 3.6.1
Grpc 1.16.0
```


### 使用

#### 1. Nuget包引用

```
Install-Package Sodao.Core.Grpc -Version 1.0.0
```


#### 2. 配置信息

##### （1）服务端配置信息

> * 支持默认配置文件appsettings.json [Consul节点可不要，如无则不是集群]

```
{
  "GrpcServer": {
    "Service": {
      "Name": "SodaoGrpcServiceApp",                    服务名称使用服务名称去除点：SodaoGrpcServiceApp
      "Host": "service.g.lan",                          专用注册的域名 （可选）
      "HostEnv": "serviceaddress",                      环境变量配置（可选，同上）
      "Port": 10001,                                    端口：与端田申请
      "Consul": {
        "Path": "dllconfigs/consulsettings.json"        Consul路径
      }
    }
  }
}
```

```
// 添加section
<configSections>
  <section name="grpcServer" type="Sodao.Core.Grpc.GrpcServerSection, Sodao.Core.Grpc" />
</configSections>

// 添加节点
<grpcServer>
  <service name="SodaoGrpcServiceApp" port="10005" host="专用注册的域名 （可选）" hostEnv="环境变量配置（可选，同上）">
    <registry>
      <consul path="dllconfigs/Consul.config" />
    </registry>
  </service>
</grpcServer>
```

##### （2）客户端配置信息

> * 命名：[命名空间].dll.json 文件夹(dllconfigs)

```
{
  "GrpcClient": {
    "Service": {
      "Name": "grpcservice",                        服务名称与服务端保持一致
      "MaxRetry":  0,                               最大可重试次数，默认不重试
      "Discovery": {
        "EndPoints": [                              单点模式
          {
            "Host": "127.0.0.1",
            "Port": 10001
          }
        ],
        "Consul": {                                 Consul 集群
          "Path": "dllconfigs/consulsettings.json"
        }
      }
    }
  }
}
```

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="grpcClient" type="Sodao.Core.Grpc.GrpcClientSection, Sodao.Core.Grpc"/>
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

> * 命名：consulsettings.json 不要改动

```
{
  "ConsulServer": {
    "Service": {
      "Address": "http://consul.g.lan" // 默认8500端口
    }
  }
}
```

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="consulServer" type="Sodao.Core.Grpc.ConsulServerSection, Sodao.Core.Grpc"/>
  </configSections>
  <consulServer>
    <service address="http://consul.g.lan"></service>
  </consulServer>
</configuration>
```


#### 3. 服务端的使用

#### （1）NetCore

> * 强制依赖注入模式

```
services.AddSingleton<GrpcExampleService.GrpcExampleServiceBase, GrpcExampleServiceImpl>();          Grpc服务的实现
services.AddSingleton<IHostedService, GrpcExampleHostedService>();                                   Grpc服务启动服务类：如下
services.AddGrpcTracer<ConsoleTracer>();                                                             Grpc注入拦截器，继承IServerTracer
```

```
using Microsoft.Extensions.Hosting;
using Sodao.Core.Grpc;
using Sodao.GrpcExample.Service.Grpc;
using System.Threading;
using System.Threading.Tasks;

namespace Sodao.GrpcService.App
{
    public class GrpcService : IHostedService
    {
        GrpcExampleService.GrpcExampleServiceBase _grpcServiceBase;
        IServerTracer _tracer;
        public GrpcService(GrpcExampleService.GrpcExampleServiceBase serviceBase, IServerTracer tracer)         依赖注入Grpc服务基础类
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

> * 实现类写法

```
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

> * 直接调用GrpcServiceManager来启动

```
using Grpc.Core;
using Sodao.Core.Grpc;
using Sodao.Log;
using System;

namespace Sodao.GrpcService
{
    public class MainService
    {
        public MainService()
        {
            
        }
        public void Start(string serviceName)               启动服务
        {
            GrpcServiceManager.Start(Library.GrpcService.BindService(new GrpcServiceImpl()), new ConsoleTracer(), (ex) =>
            {
                LogHelper.Info("", ex);
            });
        }

        public void Stop(string serviceName)                停止服务
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

> * 强制依赖注入模式
> * 配置文件默认使用    [命名空间].dll.json     可通过vs.menu工具生成nuget包
> * 注入中直接调用如下


```
// 注入Grpc客户端
services.AddGrpcClient();

// 自定义配置文件 / 默认使用命名空间.dll.json
services.Configure<GrpcClientOptions<GrpcExampleServiceClient>>((cfg) =>
{
    cfg.JsonFile = "dllconfig/Sodao.GrpcExample.Service.Grpc.dll.json";  // 可不传递
});


// 获取注入的对象
IGrpcClient<GrpcExampleServiceClient> _grpcClient;
public IndexModel(IGrpcClient<GrpcExampleServiceClient> grpcClient)
{
    _grpcClient = grpcClient;
}


var res = _grpcClient.Client.Ask(new Service.Grpc.AskRequest() { Key = "abc" });
```

#### （2）Framwork

> * 客户端代理类，编译在Dll中，类似于ThriftProxy，源码如下，可忽略

```
using Sodao.Core.Grpc;
using System;
using System.IO;

namespace Sodao.GrpcService.Generate
{
    public class ClientManager
    {
        private volatile static GrpcService.GrpcServiceClient _Client = null;
        private static readonly object lockHelper = new object();
        public static IClientTracer Tracer { get; set; } = default(IClientTracer);
        /// <summary>
        /// 单例实例
        /// </summary>
        public static GrpcService.GrpcServiceClient Instance
        {
            get
            {
                if (_Client == null)
                {
                    lock (lockHelper)
                    {
                        try
                        {
                            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dllconfigs/Sodao.GrpcService.Library.dll.config");
                            _Client = GrpcClientManager<GrpcService.GrpcServiceClient>.Get(configPath, Tracer);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"{ex.InnerException?.InnerException?.Message}");
                        }
                    }
                }
                return _Client;
            }
        }
    }
}
```

> * 使用代理类执行

```
ClientManager.Instance.[Method]
```



---
