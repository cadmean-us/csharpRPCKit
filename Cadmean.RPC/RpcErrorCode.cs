namespace Cadmean.RPC
{
    public enum RpcErrorCode
    {
        NoError = 0,
        FunctionNotCallable,
        InvalidParameters,
        InternalServerError,
        EncodingError,
        TransportError,
        NiceError = 69,
        TeapotError = 418,
        CustomError = 100,
    }
}