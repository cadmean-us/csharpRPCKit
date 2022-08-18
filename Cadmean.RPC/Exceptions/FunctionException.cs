namespace Cadmean.RPC.Exceptions;

/// <summary>
/// Represents error while calling RPC function
/// </summary>
public class FunctionException : RpcException
{
    public readonly string Code;
        
    public FunctionException(string code) : base($"Function exited with code {code}")
    {
        Code = code;
    }

    internal FunctionException(RpcErrorCode code) : base($"Function exited with code {code.Description()}")
    {
        Code = code.Description();
    }

    internal FunctionException(string code, string msg) : base(msg)
    {
        Code = code;
    }
}