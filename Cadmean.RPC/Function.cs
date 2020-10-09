using System.Collections.Generic;
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
            var output = server.Configuration.Codec.Decode<FunctionOutput>(responseData);
            ProcessMetaData(output);
            return output;
        }

        public async Task<FunctionOutput<TResult>> Call<TResult>(params object[] functionArguments)
        {
            var responseData = await ConstructCallAndSend(functionArguments);
            var output = server.Configuration.Codec.Decode<FunctionOutput<TResult>>(responseData);
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
            var data = server.Configuration.Codec.Encode(call);
            var url = $"{server.Url}/{server.Configuration.FunctionUrlProvider.GetUrl(this)}";
            return await server.Configuration.Transport.Send(
                url, 
                data, 
                server.Configuration.Codec.ContentType
            );
        }

        private void AuthorizeCallIfPossible(FunctionCall call)
        {
            call.Authorization = server.Configuration.AuthorizationTicketHolder?.Ticket.AccessToken;
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
                server.Configuration.AuthorizationTicketHolder.Ticket = ticket;
            }
        }

        private void ProcessMetaData<T>(FunctionOutput<T> output)
        {
            var meta = output.MetaData;
            if (meta == null) return;

            if (meta["authentication-success"] is bool b && b && 
                output.Result is JwtAuthorizationTicket ticket)
            {
                server.Configuration.AuthorizationTicketHolder.Ticket = ticket;
            }
        }
    }
}