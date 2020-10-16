using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Mvc;

namespace ExampleRPCServer.Controllers
{
    [ApiController]
    [Route("api/rpc/sum")]
    public class SumController : FunctionController
    {
        public int OnCall(int a, int b)
        {
            return a + b;
        }
    }
}