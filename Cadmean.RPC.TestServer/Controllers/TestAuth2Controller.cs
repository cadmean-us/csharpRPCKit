using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Cadmean.CoreKit.Authentication;
using Cadmean.RPC.ASP;
using Cadmean.RPC.ASP.Attributes;

namespace Cadmean.RPC.TestServer.Controllers
{
    [FunctionRoute("test.auth2")]
    public class TestAuth2Controller : FunctionController
    {
        public Task<JwtAuthorizationTicket> OnCall(string email, string password)
        {
            if (email != "krit.allyosha@gmail.com" || password != "bruh") 
                throw new FunctionException("invalid_credentials");
            
            var accessToken = new JwtToken(JwtAuthorizationOptions.Default, new List<Claim>(), "cadmean");
            var refreshToken = new JwtToken(JwtAuthorizationOptions.Default, new List<Claim>(), "cadmean");
            return Task.FromResult(new JwtAuthorizationTicket(accessToken.ToString(), refreshToken.ToString()));
        }
    }
}