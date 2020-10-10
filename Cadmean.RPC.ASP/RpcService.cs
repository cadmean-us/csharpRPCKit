using System.Collections.Generic;
using System.Reflection;

namespace Cadmean.RPC.ASP
{
    public class RpcService
    {
        internal readonly RpcServerConfiguration Configuration;
        
        private readonly Dictionary<string, MethodInfo> functionMethodsCache = new Dictionary<string, MethodInfo>();


        internal RpcService()
        {
            Configuration = new RpcServerConfiguration();
        }

        internal RpcService(RpcServerConfiguration configuration)
        {
            Configuration = configuration;
        }


        internal void CacheFunction(string name, MethodInfo methodInfo)
        {
            if (functionMethodsCache.ContainsKey(name)) return;
            
            functionMethodsCache.Add(name, methodInfo);
        }

        internal MethodInfo GetCachedFunction(string name)
        {
            return functionMethodsCache.ContainsKey(name) ? functionMethodsCache[name] : null;
        }
    }
}