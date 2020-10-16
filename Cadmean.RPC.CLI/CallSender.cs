using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cadmean.RPC.CLI
{
    internal static class CallSender
    {
        internal static async Task Send(string serverUrl, string functionName, 
            string[] functionArgs)
        {
            var client = new RpcClient(serverUrl);

            try
            {
                Console.WriteLine($"Calling function {functionName} at ${serverUrl}");
                var output = await client.Function(functionName).Call(ProcessArgs(functionArgs));
                Console.WriteLine("Call finished");
                Console.WriteLine($"Error: {output.Error}");
                Console.WriteLine($"Result {output.Result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Call failed with exception");
                Console.WriteLine(ex);
            }
        }

        private static object[] ProcessArgs(string[] stringArgs)
        {
            return stringArgs.Select<string, object>(strArg =>
            {
                if (long.TryParse(strArg, out long l))
                {
                    return l;
                }
                
                if (double.TryParse(strArg, out double d))
                {
                    return d;
                }

                return strArg;
            }).ToArray();
        }
    }
}