using System.Threading.Tasks;
using Cadmean.RPC.ASP;
using Cadmean.RPC.Tests;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("api/rpc/test.greetUser")]
    public class TestGreetUserController : FunctionController
    {
        private Task<string> OnCall(PocoRpcFunctionTests.User user)
        {
            return Task.FromResult($"Hello, {user.Name}{user.Age}");
        }
    }
}