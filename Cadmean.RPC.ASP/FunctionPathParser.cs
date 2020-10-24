namespace Cadmean.RPC.ASP
{
    internal static class FunctionPathParser
    {
        internal static bool IsValidRpcPath(string path, RpcServerConfiguration configuration)
        {
            path = RemoveLeadingSlash(path);
            if (!path.StartsWith(configuration.FunctionNamePrefix))
                return false;
           
            var tokens = path.Split('/');
            if (tokens.Length < 3)
                return false;

            return true;
        }

        internal static string GetFunctionName(string path, RpcServerConfiguration configuration)
        {
            path = RemoveLeadingSlash(path);
            var tokens = path.Split('/');
            return tokens[2];
        }

        private static string RemoveLeadingSlash(string path)
        {
            return path.StartsWith("/") ? path.Substring(1) : path;
        }
    }
}