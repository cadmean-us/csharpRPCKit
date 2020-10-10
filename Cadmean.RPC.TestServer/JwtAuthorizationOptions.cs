using Cadmean.CoreKit.Authentication;

namespace Cadmean.RPC.TestServer
{
    public static class JwtAuthorizationOptions
    {
        public static AuthOptions Default = new AuthOptions
        (
            "localhost:5001", 
            "localhost:5001", 
            "bruebukljhuih91283u", 
            60
        );
    }
}