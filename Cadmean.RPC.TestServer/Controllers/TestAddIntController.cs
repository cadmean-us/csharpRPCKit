using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("api/rpc/test.addInt")]
    public class TestAddInt : FunctionController
    {
        private int OnCall(int a, int b)
        {
            return a + b;
        }
    }
}