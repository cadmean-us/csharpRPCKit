using System.Threading.Tasks;
using Cadmean.RPC.Exceptions;
using Newtonsoft.Json.Linq;

namespace Cadmean.RPC
{
    /// <summary>
    /// Reference to an RPC function
    /// </summary>
    public class Function
    {
        public readonly string Name;
        private readonly RpcClient client;

        internal Function(string name, RpcClient client)
        {
            Name = name;
            this.client = client;
        }

        /// <summary>
        /// Makes a call to the RPC function.
        /// </summary>
        /// <param name="functionArguments">Arguments to be passed to the RPC function</param>
        /// <returns><c>FunctionOutput</c> that contains the result and the error code.</returns>
        /// <seealso cref="FunctionOutput"/>
        public async Task<FunctionOutput> Call(params object[] functionArguments)
        {
            var responseData = await ConstructCallAndSend(functionArguments);
            var output = client.Configuration.Codec.Decode<FunctionOutput>(responseData);
            ProcessMetaData(output);
            return output;
        }

        /// <summary>
        /// Makes a call to the RPC function.
        /// </summary>
        /// <param name="functionArguments">Arguments to be passed to the RPC function</param>
        /// <typeparam name="TResult">The type of result object of the returned <c>FunctionOutput</c></typeparam>
        /// <returns><c>FunctionOutput</c> that contains the result and the error code.</returns>
        /// <seealso cref="FunctionOutput"/>
        public async Task<FunctionOutput<TResult>> Call<TResult>(params object[] functionArguments)
        {
            var responseData = await ConstructCallAndSend(functionArguments);
            var output = client.Configuration.Codec.Decode<FunctionOutput<TResult>>(responseData);
            ProcessMetaData(output);
            return output;
        }

        /// <summary>
        /// Makes a call to the RPC function. Throws <c>FunctionException</c> if call fails.
        /// </summary>
        /// <param name="functionArguments">Arguments to be passed to the RPC function</param>
        /// <typeparam name="TResult">The type of result object</typeparam>
        /// <returns>The result returned from RPC function as <c>TResult</c></returns>
        /// <exception cref="FunctionException">Call failed with some error code.</exception>
        public async Task<TResult> CallThrowing<TResult>(params object[] functionArguments)
        {
            var output = await Call<TResult>(functionArguments);
            if (!string.IsNullOrEmpty(output.Error))
            {
                throw new FunctionException(output.Error);
            }

            return output.Result;
        }
        
        /// <summary>
        /// Makes a call to the RPC function. Throws <c>FunctionException</c> if call fails.
        /// </summary>
        /// <param name="functionArguments">Arguments to be passed to the RPC function</param>
        /// <returns></returns>
        /// <exception cref="FunctionException">Call failed with some error code.</exception>
        public async Task CallThrowing(params object[] functionArguments)
        {
            var output = await Call(functionArguments);
            if (!string.IsNullOrEmpty(output.Error))
            {
                throw new FunctionException(output.Error);
            }
        }

        public async Task Subscribe()
        {
            //todo use ISubscruber
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

            if (meta.ContainsKey("resultType") &&
                meta["resultType"] is string s && s == RpcDataType.AuthTicket && 
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

            if (meta.ContainsKey("resultType") &&
                meta["resultType"] is string s && s == RpcDataType.AuthTicket && 
                output.Result is JwtAuthorizationTicket ticket)
            {
                client.Configuration.AuthorizationTicketHolder.Ticket = ticket;
            }
        }
    }
}