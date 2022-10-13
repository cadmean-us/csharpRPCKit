using System.Linq;
using System.Threading.Tasks;
using Cadmean.RPC.CLI.Commands;

namespace Cadmean.RPC.CLI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        CommandsList.Init();
        
        var commandName = args.Length == 0 ? "help" : args[0];
        var command = CommandsList.All.FirstOrDefault(c => c.Name == commandName) ?? 
                      CommandsList.All.First(c => c.Name == "help");

        await command.Execute(args.Skip(1).ToArray());
    }
}