//using Overt.GrpcExampleNet45.Client.Tracer;
using Overt.GrpcExampleNet45.Client.Tracer;
using System;

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
                    //var client = GrpcExample.Service.Grpc.ClientManager.CreateInstance((invokers) => { return invokers[0]; });
                    GrpcExample.Service.Grpc.ClientManager.Tracer = new ConsoleTracer();
                    var client = GrpcExample.Service.Grpc.ClientManager.Instance;
                    var res = client.Ask(new GrpcExample.Service.Grpc.AskRequest() { Key = "abc" });
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
