using System.Collections.Generic;
using System.Threading.Tasks;

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
            }; 
            AuthorizeCallIfPossible(call);
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

        private void AuthorizeCallIfPossible(FunctionCall call)
        {
            call.Authorization = client.Configuration.AuthorizationTicketHolder?.Ticket.AccessToken;
        }

        private void ProcessMetaData(FunctionOutput output)
        {
            var meta = output.MetaData;
            if (meta == null) return;

            if (meta["authentication-success"] is bool b && b && 
                output.Result is Dictionary<string, object> ticketDictionary && 
                ticketDictionary.ContainsKey("accessToken") && ticketDictionary["accessToken"] is string accessToken &&
                ticketDictionary.ContainsKey("refreshToken") && ticketDictionary["refreshToken"] is string refreshToken)
            {
                var ticket = new JwtAuthorizationTicket(accessToken, refreshToken);
                client.Configuration.AuthorizationTicketHolder.Ticket = ticket;
            }
        }

        private void ProcessMetaData<T>(FunctionOutput<T> output)
        {
            var meta = output.MetaData;
            if (meta == null) return;

            if (meta["authentication-success"] is bool b && b && 
                output.Result is JwtAuthorizationTicket ticket)
            {
                client.Configuration.AuthorizationTicketHolder.Ticket = ticket;
            }
        }
    }
}