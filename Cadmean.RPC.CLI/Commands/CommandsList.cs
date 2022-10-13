using System.Collections.Generic;
using Cadmean.RPC.CLI.Commands.Call;
using Cadmean.RPC.CLI.Commands.Help;

namespace Cadmean.RPC.CLI.Commands;

public static class CommandsList
{
    public static readonly List<ICommand> All = new();

    public static void Init()
    {
        All.Add(new HelpCommand());
        All.Add(new CallCommand());
    }
}