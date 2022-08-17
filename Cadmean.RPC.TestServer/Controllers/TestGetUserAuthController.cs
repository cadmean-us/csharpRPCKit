using Cadmean.RPC.ASP;
using Cadmean.RPC.Tests;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("api/rpc/test.getUserAuth")]
    [RpcAuthorize]
    public class TestGetUserAuthController : FunctionController
    {
        public PocoRpcFunctionTests.User OnCall(string email)
        {
            if (email == "krit.allyosha@gmail.com")
            {
                return new PocoRpcFunctionTests.User
                {
                    Name = "Georg",
                    Surname = "Kot",
                    Age = 69
                };
            }
            
            throw new FunctionException("user_not_found");
        }
    }
}