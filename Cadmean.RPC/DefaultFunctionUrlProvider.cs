using System.Web;

namespace Cadmean.RPC
{
    public class DefaultFunctionUrlProvider : IFunctionUrlProvider
    {
        public string Prefix = "api/rpc";
        
        public string GetUrl(Function f)
        {
            return $"{Prefix}/{f.Name}";
        }
    }
}