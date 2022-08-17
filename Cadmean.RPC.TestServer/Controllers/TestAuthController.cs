using System.Threading.Tasks;
using Cadmean.RPC.ASP;

namespace Cadmean.RPC.TestServer.Controllers
{
    [FunctionRoute("test.auth")]
    public class TestAuthController : FunctionController
    {
        private Task<JwtAuthorizationTicket> OnCall(string email, string password)
        {
            if (email != "krit.allyosha@gmail.com" || password != "bruh") 
                throw new FunctionException("invalid_credentials");
            
            return Task.FromResult(new JwtAuthorizationTicket("access", "refresh"));
        }
    }
}