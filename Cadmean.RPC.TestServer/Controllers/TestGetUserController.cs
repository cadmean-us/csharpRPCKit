using Cadmean.RPC.ASP;
using Cadmean.RPC.Tests;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("api/rpc/test.getUser")]
    public class TestGetUserController : FunctionController
    {
        public PocoRpcFunctionTests.User OnCall()
        {
            return new PocoRpcFunctionTests.User
            {
                Name = "Alex",
                Surname = "Krit",
                Age = 42
            };
        } 
    }
}