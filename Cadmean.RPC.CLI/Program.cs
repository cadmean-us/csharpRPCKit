using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cadmean.RPC.CLI
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                await LaunchInteractiveMode();
            }
            else if (args.Length < 3)
            {
                Console.WriteLine("At least two arguments requires - server url and function name");
            }
            else
            {
                var serverName = args[0];
                var functionName = args[1];
                var functionArgs = args.Skip(2).ToArray();
                await CallSender.Send(serverName, functionName, functionArgs);
            }
        }


        private static async Task LaunchInteractiveMode()
        {
            Console.Write("Enter server url: ");
            var serverName = Console.ReadLine();
            if (serverName == "")
                serverName = "http://localhost:5000";
            Console.Write("Enter function name: ");
            var functionName = Console.ReadLine();
            Console.Write("Enter function arguments: ");
            var args = Console.ReadLine()?.Split(' ');
            await CallSender.Send(serverName, functionName, args);
            Console.WriteLine("Bye!");
        }
    }
}