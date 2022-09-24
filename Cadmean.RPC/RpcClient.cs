namespace Cadmean.RPC
{
    public class RpcClient
    {
        public readonly string ServerUrl;
        public readonly RpcConfiguration Configuration = RpcConfiguration.Default;

        public RpcClient(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        public RpcClient(string serverUrl, RpcConfiguration configuration)
        {
            ServerUrl = serverUrl;
            Configuration = configuration;
        }

        public Function Function(string name)
        {
            return new Function(name, this);
        }
    }
}