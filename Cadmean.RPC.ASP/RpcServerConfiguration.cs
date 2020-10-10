using System;

namespace Cadmean.RPC.ASP
{
    public class RpcServerConfiguration
    {
        public const int SupportedCadmeanRpcVersion = 2;
        
        public string FunctionNamePrefix = "/api/rpc";
        
        public bool AlwaysIncludeMetadata;
        
        public IAuthorizationTokenValidator AuthorizationTokenValidator;
        public bool IsAuthorizationEnabled => AuthorizationTokenValidator != null;
        
        
        public void UseAuthorization(Func<string, bool> validator)
        {
            AuthorizationTokenValidator = new DelegateAuthorizationTokenValidator(validator);
        }
    }
}