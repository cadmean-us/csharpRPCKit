using System;
using System.Threading.Tasks;

namespace Cadmean.RPC.CLI.Commands.Help;

public class HelpCommand : ICommand
{
    public string Name => "help";

    public string Description => "Shows help";
    
    public async Task Execute(string[] args)
    {
        Console.WriteLine(@"
 ________  ________  ________  _____ ______   _______   ________  ________       ________  ________  ________     
|\   ____\|\   __  \|\   ___ \|\   _ \  _   \|\  ___ \ |\   __  \|\   ___  \    |\   __  \|\   __  \|\   ____\    
\ \  \___|\ \  \|\  \ \  \_|\ \ \  \\\__\ \  \ \   __/|\ \  \|\  \ \  \\ \  \   \ \  \|\  \ \  \|\  \ \  \___|    
 \ \  \    \ \   __  \ \  \ \\ \ \  \\|__| \  \ \  \_|/_\ \   __  \ \  \\ \  \   \ \   _  _\ \   ____\ \  \       
  \ \  \____\ \  \ \  \ \  \_\\ \ \  \    \ \  \ \  \_|\ \ \  \ \  \ \  \\ \  \ __\ \  \\  \\ \  \___|\ \  \____  
   \ \_______\ \__\ \__\ \_______\ \__\    \ \__\ \_______\ \__\ \__\ \__\\ \__\\__\ \__\\ _\\ \__\    \ \_______\
    \|_______|\|__|\|__|\|_______|\|__|     \|__|\|_______|\|__|\|__|\|__| \|__\|__|\|__|\|__|\|__|     \|_______|
                                                                                                                  ");
        
        Console.WriteLine(@"Usage:
cadrpc <command> [args]
");

        Console.WriteLine("Available commands:\n");
        foreach (var command in CommandsList.All)
        {
            Console.WriteLine($"{command.Name} - {command.Description}");
        }
        
        Console.WriteLine(@"
Create and apply migration (PostgreSQL):
dotnet ef migrations add <migration name> --project <project name>
dotnet ef database update --connection ""Host=127.0.0.1;Username=ubunut;Password=\!Devpassword1;Database=<dn name>"" --project <project name>
");
    }
}