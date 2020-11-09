namespace Cadmean.RPC
{
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
}