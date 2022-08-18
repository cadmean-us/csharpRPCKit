namespace Cadmean.RPC.Exceptions;

public class RpcFeatureNotSupportedException : RpcException
{
    public RpcFeatureNotSupportedException(string feature) : base($"Feature '{feature}' is not supported in current client or server implementation")
    {
    }
}