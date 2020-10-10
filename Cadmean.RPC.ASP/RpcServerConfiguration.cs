namespace Cadmean.RPC.ASP
{
    public struct RpcServerConfiguration
    {
        public const int SupportedCadmeanRpcVersion = 2;
        public string FunctionNamePrefix;
        public bool AlwaysIncludeMetadata;

        public static RpcServerConfiguration Default = new RpcServerConfiguration
        {
            FunctionNamePrefix = "/api/rpc"
        };
    }
}