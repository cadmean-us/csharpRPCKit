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
            var responseData = await ConstructCallAndSend(functionArguments);
            return server.Configuration.Codec.Decode<FunctionOutput>(responseData);
        }

        public async Task<FunctionOutput<TResult>> Call<TResult>(params object[] functionArguments)
        {
            var responseData = await ConstructCallAndSend(functionArguments);
            return server.Configuration.Codec.Decode<FunctionOutput<TResult>>(responseData);
        }

        private async Task<byte[]> ConstructCallAndSend(object[] functionArguments)
        {
            var call = new FunctionCall
            {
                Arguments = functionArguments,
            };
            if (server.Configuration.AuthorizationProvider != null)
            {
                call.Authorization = server.Configuration.AuthorizationProvider.Authorize();
            }
            return await SendCall(call);
        }

        private async Task<byte[]> SendCall(FunctionCall call)
        {
            var data = server.Configuration.Codec.Encode(call);
            var url = $"{server.Url}/{server.Configuration.FunctionUrlProvider.GetUrl(this)}";
            return await server.Configuration.Transport.Send(
                url, 
                data, 
                server.Configuration.Codec.ContentType
            );
        }
    }
}