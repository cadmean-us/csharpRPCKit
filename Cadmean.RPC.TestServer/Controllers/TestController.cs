using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("/api/v1/test")]
    public class TestController : FunctionController
    {

        int OnCall(int a, int b)
        {
            return a + b;
        }
    }
}