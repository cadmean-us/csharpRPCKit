using System;

namespace Cadmean.RPC.ASP
{
    public class FunctionException : Exception
    {
        public readonly int Code;
        
        public FunctionException(int code) : base($"Function exited with code {code}")
        {
            Code = code;
        }
    }
}