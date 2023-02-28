using System;
using System.Collections.Generic;

namespace Cadmean.RPC.ASP;

public class RpcService
{
    internal readonly RpcServerConfiguration Configuration;
        
    private readonly Dictionary<string, CachedFunctionInfo?> functionInfoCache = new();

    internal RpcService()
    {
        Configuration = new RpcServerConfiguration();
        AnalyzeFunctions();
    }

    internal RpcService(RpcServerConfiguration configuration)
    {
        Configuration = configuration;
        AnalyzeFunctions();
    }

    private void AnalyzeFunctions()
    {
        var analyzer = new FunctionsAnalyzer(Configuration);
        foreach (var info in analyzer.GetFunctionInfos())
        {
            functionInfoCache.Add(info.Name, info);
        }
    }

    internal CachedFunctionInfo? GetCachedFunctionInfo(string name)
    {
        return functionInfoCache.ContainsKey(name) ? functionInfoCache[name] : null;
    }

    internal void InvokeExceptionHandler(Exception ex)
    {
        Configuration.ExceptionHandler?.Invoke(ex);
    }
}