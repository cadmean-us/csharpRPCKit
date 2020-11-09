namespace Cadmean.RPC
{
    /// <summary>
    /// Represents error while calling RPC function
    /// </summary>
    public class FunctionException : RpcException
    {
        public readonly int Code;
        
        public FunctionException(ushort code) : base($"Function exited with code {code}")
        {
            Code = code;
        }
        
        internal FunctionException(int code, string msg) : base(msg)
        {
            Code = code;
        }
    }
}