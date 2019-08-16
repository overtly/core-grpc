//using Overt.GrpcExampleNet45.Client.Tracer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Overt.GrpcExample.Service.Grpc.GrpcExampleService;
using static Overt.GrpcExample.Service.Grpc.GrpcExampleService1;

namespace Overt.GrpcExampleNet45.Client
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
                    //Overt.GrpcExample.Service.Grpc.ClientManager.ConfigPath = "dllconfigs/aa.config";
                    var res = Overt.GrpcExample.Service.Grpc.ClientManager.Instance.Ask(new GrpcExample.Service.Grpc.AskRequest() { Key = "abc" });
                    Console.WriteLine("例子：" + res?.Content ?? "abc");

                    Overt.GrpcExample.Service.Grpc.ClientManager.Configure<GrpcExampleService1Client>("dllconfigs/Overt.GrpcExample.Service.Grpc.1.dll.config");
                    var res1 = GrpcExample.Service.Grpc.ClientManager<GrpcExampleService1Client>.Instance.Ask(new GrpcExample.Service.Grpc.AskRequest() { Key = "ccc" });
                    Console.WriteLine("例子：" + res1?.Content ?? "abc");
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
