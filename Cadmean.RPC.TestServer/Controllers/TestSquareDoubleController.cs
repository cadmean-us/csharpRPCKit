using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("api/rpc/test.squareDouble")]
    public class TestSquareDoubleController : FunctionController
    {
        private double OnCall(double a)
        {
            return a * a;
        }
    }
}