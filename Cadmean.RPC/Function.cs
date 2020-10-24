using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Cadmean.RPC
{
    public class Function
    {
        public readonly string Name;
        private readonly RpcClient client;

        internal Function(string name, RpcClient client)
        {
            Name = name;
            this.client = client;
        }

        public async Task<FunctionOutput> Call(params object[] functionArguments)
        {
            var responseData = await ConstructCallAndSend(functionArguments);
            var output = client.Configuration.Codec.Decode<FunctionOutput>(responseData);
            ProcessMetaData(output);
            return output;
        }

        public async Task<FunctionOutput<TResult>> Call<TResult>(params object[] functionArguments)
        {
            var responseData = await ConstructCallAndSend(functionArguments);
            var output = client.Configuration.Codec.Decode<FunctionOutput<TResult>>(responseData);
            ProcessMetaData(output);
            return output;
        }

        private async Task<byte[]> ConstructCallAndSend(object[] functionArguments)
        {
            var call = new FunctionCall
            {
                Arguments = functionArguments,
                Authorization = AuthorizeCallIfPossible()
            };
            return await SendCall(call);
        }

        private async Task<byte[]> SendCall(FunctionCall call)
        {
            var data = client.Configuration.Codec.Encode(call);
            var url = $"{client.ServerUrl}/{client.Configuration.FunctionUrlProvider.GetUrl(this)}";
            return await client.Configuration.Transport.Send(
                url, 
                data, 
                client.Configuration.Codec.ContentType
            );
        }

        private string AuthorizeCallIfPossible()
        {
            return client.Configuration.AuthorizationTicketHolder?.Ticket.AccessToken;
        }

        private void ProcessMetaData(FunctionOutput output)
        {
            var meta = output.MetaData;
            if (meta == null) return;

            if (meta.ContainsKey("clrResultType") &&
                meta["clrResultType"] is string s && s == "Cadmean.RPC.JwtAuthorizationTicket" && 
                output.Result is JObject json && 
                json.ContainsKey("accessToken") && json.GetValue("accessToken")?.Value<string>() is { } accessToken &&
                json.ContainsKey("refreshToken") && json.GetValue("refreshToken")?.Value<string>() is { } refreshToken)
            {
                var ticket = new JwtAuthorizationTicket(accessToken, refreshToken);
                client.Configuration.AuthorizationTicketHolder.Ticket = ticket;
            }
        }

        private void ProcessMetaData<T>(FunctionOutput<T> output)
        {
            var meta = output.MetaData;
            if (meta == null) return;

            if (meta.ContainsKey("clrResultType") &&
                meta["clrResultType"] is string s && s == "Cadmean.RPC.JwtAuthorizationTicket" && 
                output.Result is JwtAuthorizationTicket ticket)
            {
                client.Configuration.AuthorizationTicketHolder.Ticket = ticket;
            }
        }
    }
}