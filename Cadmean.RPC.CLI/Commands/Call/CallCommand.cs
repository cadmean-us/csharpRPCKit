using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cadmean.RPC.CLI.Commands.Call;

public class CallCommand : ICommand
{
    private bool repeat;
        
    private string serverName = "";
    private string functionName = "";
    private string[] functionArgs;
    
    public string Name => "call";

    public string Description => "Call an RPC function on an RPC server with given arguments";
    
    public async Task Execute(string[] args)
    {
        if (args.Length == 0)
        {
            await LaunchInteractiveMode();
            return;
        }

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
                            serverName = ResolveSpecialServerName(arg);
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
    
    private async Task LaunchInteractiveMode()
    {
        while (true)
        {
            if (string.IsNullOrEmpty(serverName))
            {
                Console.Write("Enter server url: ");
                var sn = Console.ReadLine();
                serverName = ResolveSpecialServerName(sn);
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

    private static string ResolveSpecialServerName(string sn)
    {
        return sn switch
        {
            "" => "http://localhost:5000",
            ":80" => "http://localhost:80",
            ":5000" => "http://localhost:5000",
            _ => sn
        };
    }
}