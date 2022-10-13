using System.Threading.Tasks;
using Cadmean.RPC.ASP;
using Cadmean.RPC.ASP.Attributes;

namespace Cadmean.RPC.TestServer.Controllers
{
    [FunctionRoute("test.auth")]
    public class TestAuthController : FunctionController
    {
        private Task<JwtAuthorizationTicket> OnCall(string email, string password)
        {
            if (email != "email@example.com" || password != "password") 
                throw new FunctionException("invalid_credentials");
            
            return Task.FromResult(new JwtAuthorizationTicket("access", "refresh"));
        }
    }
}