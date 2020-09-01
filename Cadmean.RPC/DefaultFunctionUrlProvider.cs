using System.Web;

namespace Cadmean.RPC
{
    public class DefaultFunctionUrlProvider : IFunctionUrlProvider
    {
        public string Prefix = "";
        
        public string GetUrl(Function f)
        {
            return $"/{HttpUtility.UrlEncode(Prefix)}/{f.Name}";
        }
    }
}