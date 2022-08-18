using System;
using System.Threading.Tasks;

namespace Cadmean.RPC;

public interface IFunctionSubscriber
{
    public Task Subscribe<TResult>(string serverName, string functionName, Action<TResult> callback);
}