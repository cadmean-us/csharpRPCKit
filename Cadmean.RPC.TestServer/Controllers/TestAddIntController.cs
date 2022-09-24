using Cadmean.RPC.ASP;
using Cadmean.RPC.ASP.Attributes;

namespace Cadmean.RPC.TestServer.Controllers
{
    [FunctionRoute("test.addInt")]
    public class TestAddIntController : FunctionController
    {
        private int OnCall(int a, int b)
        {
            return a + b;
        }
    }
}