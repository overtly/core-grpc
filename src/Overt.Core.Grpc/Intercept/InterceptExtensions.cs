namespace Overt.Core.Grpc.Intercept
{
    public static class InterceptExtensions
    {
        /// <summary>
        /// ClientIntercept注入
        /// </summary>
        /// <param name="callInvoker"></param>
        /// <param name="tracer"></param>
        /// <returns></returns>
        public static ServerCallInvoker ClientIntercept(this ServerCallInvoker callInvoker, IClientTracer tracer)
        {
            if (callInvoker == null)
                return callInvoker;

            return new InterceptedServerCallInvoker(callInvoker.Channel, tracer);
        }
    }
}
