using System;

namespace Cadmean.RPC.ASP
{
    public class RpcServerException : Exception
    {
        public RpcServerException(string message) : base(message)
        {
        }
    }
}