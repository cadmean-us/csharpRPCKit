using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.ASP
{
    internal class FunctionsAnalyzer
    {
        private readonly RpcServerConfiguration rpcServerConfiguration;

        public FunctionsAnalyzer(RpcServerConfiguration rpcServerConfiguration)
        {
            this.rpcServerConfiguration = rpcServerConfiguration;
        }

        internal List<CachedFunctionInfo> GetFunctionInfos()
        {
            var infos = new List<CachedFunctionInfo>();
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var controllerType in GetFunctionControllerDerivedTypes(assembly))
                {
                    var info = AnalyzeFunctionControllerType(controllerType);
                    if (info != null)
                        infos.Add(info);
                }
            }
            
            LogFoundFunctions(infos);
            
            return infos;
        }

        private static IEnumerable<Type> GetFunctionControllerDerivedTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(FunctionController)));
        }

        private CachedFunctionInfo AnalyzeFunctionControllerType(Type controllerType)
        {
            var method = FindOnCallMethod(controllerType);
            if (method == null) return null;

            var path = ResolveRoute(controllerType);

            if (!FunctionRouteParser.IsValidRpcRoute(path, rpcServerConfiguration))
                return null;

            var name = FunctionRouteParser.GetFunctionName(path, rpcServerConfiguration);
            if (!FunctionRouteParser.IsValidFunctionName(name))
                return null;

            var info = new CachedFunctionInfo
            {
                CallMethod = method,
                Name = name,
                Path = path,
                IsCallMethodAsync = IsMethodAsync(method),
                RequiresAuthorization = FunctionRequiresAuthorization(method),
            };

            return info;
        }

        private static string ResolveRoute(Type controllerType)
        {
            if (controllerType.GetCustomAttribute(typeof(RouteAttribute)) is RouteAttribute route)
            {
                return route.Template;
            }

            if (controllerType.GetCustomAttribute(typeof(RouteAttribute)) is FunctionRouteAttribute functionRoute)
            {
                return functionRoute.FullPath;
            }

            return null;
        }

        private static MethodInfo FindOnCallMethod(Type controllerType)
        {
            return controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(m => 
                    (m.Name == "OnCall" || m.GetCustomAttribute(typeof(OnCallAttribute)) != null) && 
                    !m.IsVirtual && !m.IsAbstract && !m.IsConstructor && !m.IsStatic
                );
        }
        
        private bool IsMethodAsync(MethodInfo callMethod)
        {
            return callMethod.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
        }
        
        private bool FunctionRequiresAuthorization(MethodInfo methodInfo)
        {
            if (!rpcServerConfiguration.IsAuthorizationEnabled)
                return false;
            
            var attributes = methodInfo.GetCustomAttributes();
            return attributes.Any(attr =>
                attr.GetType() == typeof(RpcAuthorizeAttribute) || attr.GetType() == typeof(AuthorizeAttribute));
        }


        private void LogFoundFunctions(List<CachedFunctionInfo> infos)
        {
            if (!rpcServerConfiguration.DebugMode)
                return;
            
            Console.WriteLine($"Found {infos.Count} functions:");
            foreach (var info in infos)
            {
                Console.WriteLine($"\t{info}");
            }
        }
    }
}