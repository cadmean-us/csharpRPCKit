using System;

namespace Cadmean.RPC.ASP
{
    public class RpcServerConfiguration
    {
        public const string SupportedCadmeanRpcVersion = "2.1";
        
        public string FunctionNamePrefix = "api/rpc";
        
        public bool AlwaysIncludeMetadata;
        public bool DebugMode;
        
        public IAuthorizationTokenValidator AuthorizationTokenValidator;
        public bool IsAuthorizationEnabled => AuthorizationTokenValidator != null;
        
        
        public void UseAuthorization(Func<string, bool> validator)
        {
            AuthorizationTokenValidator = new DelegateAuthorizationTokenValidator(validator);
        }
    }
}