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
            var message = new HttpRequestMessage(HttpMethod.Post, url) {Content = content};
            var response = await client.SendAsync(message);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}