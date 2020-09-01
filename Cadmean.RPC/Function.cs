using System.Threading.Tasks;

namespace Cadmean.RPC
{
    public class Function
    {
        public readonly string Name;
        private readonly RpcServer server;

        internal Function(string name, RpcServer server)
        {
            Name = name;
            this.server = server;
        }

        public async Task<FunctionOutput> Call(params object[] functionArguments)
        {
            var call = new FunctionCall
            {
                Arguments = functionArguments,
            };

            var data = server.Configuration.Codec.Encode(call);
            var url = $"{server.Url}/{server.Configuration.FunctionUrlProvider.GetUrl(this)}";
            var responseData = await server.Configuration.Transport.Send(url, data, server.Configuration.Codec.ContentType);
            return server.Configuration.Codec.Decode<FunctionOutput>(responseData);
        }

        public async Task<FunctionOutput<TResult>> Call<TResult>(params object[] functionArguments)
        {
            var call = new FunctionCall
            {
                Arguments = functionArguments,
            };

            var data = server.Configuration.Codec.Encode(call);
            var url = $"{server.Url}/{server.Configuration.FunctionUrlProvider.GetUrl(this)}";
            var responseData = await server.Configuration.Transport.Send(url, data, server.Configuration.Codec.ContentType);
            return server.Configuration.Codec.Decode<FunctionOutput<TResult>>(responseData);
        }
    }
}