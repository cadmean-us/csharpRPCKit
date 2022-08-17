using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Cadmean.RPC
{
    public class HttpTransportProvider : ITransportProvider
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<byte[]> Send(string url, byte[] data, string contentType)
        {
            var content = new ByteArrayContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Headers.Add("Cadmean-RPC-Version", "3.0");
            
            var message = new HttpRequestMessage(HttpMethod.Post, url) {Content = content};

            HttpResponseMessage response;
            
            try
            {
                response = await client.SendAsync(message);
            }
            catch (HttpRequestException)
            {
                throw new FunctionException(RpcErrorCode.TransportError);
            }

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                throw new FunctionException(RpcErrorCode.NotSuccessfulStatusCode);
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}