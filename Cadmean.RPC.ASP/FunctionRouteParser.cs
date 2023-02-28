using System.Text.RegularExpressions;

namespace Cadmean.RPC.ASP
{
    internal static class FunctionRouteParser
    {
        internal static bool IsValidRpcRoute(string? route, RpcServerConfiguration configuration)
        {
            if (route == null)
                return false;
            
            route = RemoveLeadingSlash(route);
            if (!route.StartsWith(configuration.FunctionNamePrefix))
                return false;
           
            var tokens = route.Split('/');
            if (tokens.Length < 3)
                return false;

            return true;
        }
        
        private const string NamePattern = @"^[A-z0-9]+(.|)[A-z0-9]+$";
        internal static bool IsValidFunctionName(string name)
        {
            return Regex.IsMatch(name, NamePattern);
        }

        internal static string GetFunctionName(string route, RpcServerConfiguration configuration)
        {
            route = RemoveLeadingSlash(route);
            var tokens = route.Split('/');
            return tokens[2];
        }

        private static string RemoveLeadingSlash(string route)
        {
            return route.StartsWith("/") ? route.Substring(1) : route;
        }
    }
}