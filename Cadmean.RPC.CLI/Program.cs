using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cadmean.RPC.CLI
{
    public static class Program
    {
        private static bool repeat;
        
        private static string serverName = "";
        private static string functionName = "";
        private static string[] functionArgs;
        
        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                await LaunchInteractiveMode();
            }
            else
            {
                int i = 0;
                int c = 0;

                foreach (var arg in args)
                {
                    switch (arg)
                    {
                        case "-r":
                            repeat = true;
                            break;
                        default:
                            switch (c)
                            {
                                case 0:
                                    serverName = arg switch
                                    {
                                        "local" => "http://localhost:5000",
                                        "test" => "http://testrpc.cadmean.ru",
                                        _ => arg,
                                    };
                                    break;
                                case 1:
                                    functionName = arg;
                                    break;
                                default:
                                    functionArgs = args.Skip(i).ToArray();
                                    break;
                            }

                            c++;
                            break;
                    }

                    i++;
                }

                if (c < 2)
                    await LaunchInteractiveMode();
                else
                    await CallSender.Send(serverName, functionName, functionArgs);
            }
        }


        private static async Task LaunchInteractiveMode()
        {
            while (true)
            {
                if (string.IsNullOrEmpty(Program.serverName))
                {
                    Console.Write("Enter server url: ");
                    var sn = Console.ReadLine();
                    serverName = sn switch
                    {
                        "" => "http://localhost:5000",
                        "test" => "http://testrpc.cadmean.ru",
                        _ => sn
                    };
                }

                if (string.IsNullOrEmpty(functionName))
                {
                    Console.Write("Enter function name: "); 
                    functionName = Console.ReadLine();
                }

                Console.Write("Enter function arguments: ");
                var args = Console.ReadLine()?.Split(' ');
                await CallSender.Send(serverName, functionName, args);

                if (repeat)
                {
                    functionName = "";
                    continue;
                }
                break;
            }
        }
    }
}