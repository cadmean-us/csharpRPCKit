using Cadmean.RPC.ASP;

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