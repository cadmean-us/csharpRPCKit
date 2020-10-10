using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("api/rpc/test.concatString")]
    public class TestConcatStringController : FunctionController
    {
        private string OnCall(string a, string b, string c)
        {
            return a + b + c;
        }
    }
}