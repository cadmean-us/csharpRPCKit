namespace Cadmean.RPC;

public enum RpcErrorCode
{
    NoError = 0,
    
    FunctionNotCallable = -100,
    FunctionNotFound = -101,
    IncompatibleRpcVersion = -102,
    
    InvalidArguments = -200,
    
    EncodeError = -300,
    DecodeError = -301,
    
    TransportError = -400,
    NotSuccessfulStatusCode = -401,
    
    InternalServerError = -500,
    
    AuthorizationError = -600,
    
    PreCallChecksFailed = -700,
    
    NiceError = -69,
}

public static class RpcErrorCodeExtension
{
    public static string Description(this RpcErrorCode code)
    {
        return code switch
        {
            RpcErrorCode.NoError => "",
            RpcErrorCode.FunctionNotCallable => "function_not_callable",
            RpcErrorCode.FunctionNotFound => "function_not_found",
            RpcErrorCode.IncompatibleRpcVersion => "incompatible_rpc_version",
            RpcErrorCode.InvalidArguments => "invalid_arguments",
            RpcErrorCode.EncodeError => "encode_error",
            RpcErrorCode.DecodeError => "decode_error",
            RpcErrorCode.TransportError => "transport_error",
            RpcErrorCode.NotSuccessfulStatusCode => "not_successful_status_code",
            RpcErrorCode.InternalServerError => "internal_server_error",
            RpcErrorCode.AuthorizationError => "authorization_error",
            RpcErrorCode.PreCallChecksFailed => "pre_call_checks_failed",
            RpcErrorCode.NiceError => "nice_error",
            _ => "unknown_error",
        };
    }
}