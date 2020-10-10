using System;

namespace Cadmean.RPC
{
    /// <summary>
    /// General exception class for all RPC-related errors
    /// </summary>
    public class RpcException : Exception
    {
        public RpcException()
        {
        }

        public RpcException(string message) : base(message)
        {
        }
    }
}