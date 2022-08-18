using System.Collections.Generic;
using System.Linq;
using Cadmean.RPC.ASP;

namespace Cadmean.RPC.TestServer.Controllers;

[FunctionRoute("test.listSum")]
public class ListSumController : FunctionController
{
    public int OnCall(List<int> list)
    {
        return list.Aggregate((a, b) => a + b);
    }
}