using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Cadmean.CoreKit.Authentication;
using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("api/rpc/test.auth")]
    public class TestAuthController : FunctionController
    {
        private Task<JwtAuthorizationTicket> OnCall(string email, string password)
        {
            if (email != "krit.allyosha@gmail.com" || password != "bruh") 
                throw new FunctionException(101);
            
            return Task.FromResult(new JwtAuthorizationTicket("access", "refresh"));
        }
    }
}