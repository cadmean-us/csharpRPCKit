using System.Threading.Tasks;

namespace Cadmean.RPC
{
    public interface ITransportProvider
    {
        public Task<byte[]> Send(string url, byte[] data, string contentType);
    }
}