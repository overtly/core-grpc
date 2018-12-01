//using Sodao.GrpcExampleNet45.Client.Tracer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodao.GrpcExampleNet45.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.A)
                    break;

                try
                {
                    //var res1 = Sodao.Juketool.WeChat.CloudStorage.Service.Grpc.ClientManager.Instance.Device_GetByIMEI(new Juketool.WeChat.CloudStorage.Service.Grpc.DeviceIMEIRequest()
                    //{
                    //    IMEI = "869319020053033"
                    //});
                    //Console.WriteLine(res1?.Data?.IMEI + " - " + DateTime.Now);

                    //Sodao.GrpcExample.Service.Grpc.ClientManager.Tracer = new ConsoleTracer();
                    var res = Sodao.GrpcExample.Service.Grpc.ClientManager.Instance.Ask(new GrpcExample.Service.Grpc.AskRequest() { Key = "abc" });
                    Console.WriteLine("例子：" + res?.Content ?? "abc");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("异常");
                }

            } while (true);

            Console.WriteLine("结束...");
            Console.ReadLine();
        }
    }
}
