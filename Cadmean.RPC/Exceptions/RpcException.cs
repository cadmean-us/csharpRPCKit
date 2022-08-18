using System;

namespace Cadmean.RPC.Exceptions;

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