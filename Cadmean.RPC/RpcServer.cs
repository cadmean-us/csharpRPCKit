namespace Cadmean.RPC
{
    public class RpcServer
    {
        public readonly string Url;
        public readonly RpcConfiguration Configuration = RpcConfiguration.Default;

        public RpcServer(string url)
        {
            Url = url;
        }

        public Function Function(string name)
        {
            return new Function(name, this);
        }
    }
}