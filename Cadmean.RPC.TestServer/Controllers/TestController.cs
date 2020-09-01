using System.Threading.Tasks;
using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("/api/v1/test")]
    public class TestController : FunctionController
    {

        public Task<long> OnCall(long a, long b)
        {
            return Task.FromResult(a + b);
        }
    }
}