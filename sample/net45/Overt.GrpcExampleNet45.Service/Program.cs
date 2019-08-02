using System;
using System.Configuration;
using Topshelf;

namespace Overt.GrpcExampleNet45.Service
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            var serviceName = ConfigurationManager.AppSettings["ServiceName"];
            var serviceDescription = ConfigurationManager.AppSettings["ServiceDescription"];
            var serviceDisplayName = ConfigurationManager.AppSettings["ServiceDisplayName"];
            if (string.IsNullOrEmpty(serviceName)
                || string.IsNullOrEmpty(serviceDisplayName)
                || string.IsNullOrEmpty(serviceDescription))
                throw new ArgumentNullException();
            HostFactory.Run(x =>
            {
                x.Service<MainService>(s =>
                {
                    s.ConstructUsing(service => new MainService());
                    s.WhenStarted(ts => ts.Start(serviceName));
                    s.WhenShutdown(ts => ts.ShutDown(serviceName));
                    s.WhenStopped(ts => ts.Stop(serviceName));
                });
                x.SetServiceName(serviceName);
                x.SetDisplayName(serviceDisplayName);
                x.SetDescription(serviceDescription);
                // 启动方式
                x.StartAutomatically();
                //x.StartAutomaticallyDelayed();
                //x.StartManually();
                //x.Disabled();

                // 服务运行账户
                x.RunAsLocalSystem();
                //x.RunAsLocalSystem();
                //x.RunAsNetworkService();
                //x.RunAsPrompt();

                // 服务依赖项
                //x.DependsOn(servicenames);

                // 自动恢复（服务重启）设置
                x.EnableServiceRecovery(r =>
                {
                    var delay = 0;
                    // 第一次失败 
                    // 重启服务
                    r.RestartService(delay);

                    // 第二次失败 
                    //// 运行指定外部程序
                    //r.RunProgram(delay, "command");

                    // 第三次失败 
                    //// 重启计算机
                    //r.RestartComputer(delay, string.Format("Service {0} crashed!", serviceName));

                    // 仅服务崩溃时重启服务
                    r.OnCrashOnly();

                    // 恢复计算周期
                    r.SetResetPeriod(1);
                });
            });
        }
    }
}
