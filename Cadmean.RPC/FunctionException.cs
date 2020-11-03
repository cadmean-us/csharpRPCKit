namespace Cadmean.RPC
{
    public class FunctionException : RpcException
    {
        public readonly ushort Code;
        
        public FunctionException(ushort code) : base($"Function exited with code {code}")
        {
            Code = code;
        }
    }
}