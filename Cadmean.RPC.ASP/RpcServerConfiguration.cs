using System;

namespace Cadmean.RPC.ASP;

public class RpcServerConfiguration
{
    public const string SupportedCadmeanRpcVersion = "3.0";
        
    public string FunctionNamePrefix = "api/rpc";
        
    public bool AlwaysIncludeMetadata;
    public bool DebugMode;
        
    public IAuthorizationTokenValidator? AuthorizationTokenValidator;
    public bool IsAuthorizationEnabled => AuthorizationTokenValidator != null;

    public Action<Exception>? ExceptionHandler;

    public void UseAuthorization(Func<string, bool> validator)
    {
        AuthorizationTokenValidator = new DelegateAuthorizationTokenValidator(validator);
    }
}