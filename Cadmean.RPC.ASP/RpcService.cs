using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cadmean.RPC.ASP
{
    public class RpcService
    {
        internal readonly RpcServerConfiguration Configuration;
        
        private readonly Dictionary<string, CachedFunctionInfo> functionMethodsCache = 
            new Dictionary<string, CachedFunctionInfo>();


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
                functionMethodsCache.Add(info.Name, info);
            }
        }

        internal CachedFunctionInfo GetCachedFunctionInfo(string name)
        {
            return functionMethodsCache.ContainsKey(name) ? functionMethodsCache[name] : null;
        }
    }
}