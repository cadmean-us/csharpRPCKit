using System.Net.Http;
using System.Threading.Tasks;

namespace Cadmean.RPC
{
    public class HttpTransportProvider : ITransportProvider
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<byte[]> Send(string url, byte[] data, string contentType)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, url) {Content = new ByteArrayContent(data)};
            message.Headers.Add("Content-Type", contentType);
            var response = await client.SendAsync(message);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}