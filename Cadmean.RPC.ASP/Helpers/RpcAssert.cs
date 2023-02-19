using System;

namespace Cadmean.RPC.ASP.Helpers;

public static class RpcAssert
{
    public static void IsTrue<TException>(bool condition) where TException : FunctionException, new()
    {
        if (condition)
        {
            return;
        }

        throw new TException();
    }

    public static void IsTrue<TException>(Func<bool> condition) where TException : FunctionException, new()
    {
        IsTrue<TException>(condition());
    }
    
    public static void IsTrue(bool condition, string error)
    {
        if (condition)
        {
            return;
        }

        throw new FunctionException(error);
    }

    public static void IsTrue(Func<bool> condition, string error)
    {
        IsTrue(condition(), error);
    }
}