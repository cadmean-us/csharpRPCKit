using System.Threading.Tasks;

namespace Cadmean.RPC.CLI.Commands;

public interface ICommand
{
    string Name { get; }
    string Description { get; }

    Task Execute(string[] args);
}