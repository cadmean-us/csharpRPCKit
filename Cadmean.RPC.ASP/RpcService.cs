using System;
using System.Collections.Generic;

namespace Cadmean.RPC.ASP
{
    public class RpcService
    {
        internal readonly RpcServerConfiguration Configuration;
        
        private readonly Dictionary<string, CachedFunctionInfo> functionInfoCache = 
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
                functionInfoCache.Add(info.Name, info);
            }
        }

        internal CachedFunctionInfo GetCachedFunctionInfo(string name)
        {
            return functionInfoCache.ContainsKey(name) ? functionInfoCache[name] : null;
        }

        private readonly Dictionary<string, List<Action<FunctionOutput>>> callbacks = new();

        internal void WatchFunctionOutput(string functionName, Action<FunctionOutput> callback)
        {
            List<Action<FunctionOutput>> callbacksList;
            if (callbacks.ContainsKey(functionName))
            {
                callbacksList = callbacks[functionName];
            }
            else
            {
                callbacksList = new();
                callbacks[functionName] = callbacksList;
            }

            if (!callbacksList.Contains(callback))
            {
                callbacksList.Add(callback);
            }
        }

        internal void SubmitFunctionOutput(string functionName, FunctionOutput output)
        {
            if (!callbacks.ContainsKey(functionName))
            {
                return;
            }
            
            var callbacksList = callbacks[functionName];
            foreach (var callback in callbacksList)
            {
                callback(output);
            }
        }
    }
}