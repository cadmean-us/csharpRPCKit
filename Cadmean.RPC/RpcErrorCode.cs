namespace Cadmean.RPC
{
    public enum RpcErrorCode
    {
        NoError = 0,
        FunctionNotCallable = -100,
        FunctionNotFound = -101,
        IncompatibleRpcVersion = -102,
        InvalidParameters = -200,
        EncodingError = -300,
        TransportError = -400,
        InternalServerError = -500,
        AuthorizationError = -600,
        NiceError = -69,
    }
}