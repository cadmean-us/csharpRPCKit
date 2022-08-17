using System;

namespace Cadmean.RPC.ASP.Helpers;

public static class RpcAssert
{
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