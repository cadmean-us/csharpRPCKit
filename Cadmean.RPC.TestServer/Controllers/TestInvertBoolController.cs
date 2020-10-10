using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("api/rpc/test.invertBool")]
    public class TestInvertBoolController : FunctionController
    {
        private bool OnCall(bool b)
        {
            return !b;
        }
    }
}