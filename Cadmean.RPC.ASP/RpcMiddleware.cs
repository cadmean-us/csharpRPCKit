using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Cadmean.RPC.ASP
{
    public class RpcMiddleware
    {
        private readonly RequestDelegate next;
        private readonly RpcService rpcService;

        public RpcMiddleware(RequestDelegate next, RpcService rpcService)
        {
            this.next = next;
            this.rpcService = rpcService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var headers = request.Headers;
            var pathStr = request.Path.Value;

            if (!FunctionRouteParser.IsValidRpcRoute(pathStr, rpcService.Configuration))
            {
                await next.Invoke(context);
                return;
            }

            if (!CheckRpcHeader(headers))
            {
                await SendErrorOutput(response, RpcErrorCode.IncompatibleRpcVersion);
                return;
            }
            
            var fName = FunctionRouteParser.GetFunctionName(pathStr, rpcService.Configuration);
            var functionInfo = rpcService.GetCachedFunctionInfo(fName);

            if (functionInfo == null)
            {
                await SendErrorOutput(response, RpcErrorCode.FunctionNotFound);
                return;
            }
            
            context.Items["rpcService"] = rpcService;
            context.Items["functionInfo"] = functionInfo;

            await next.Invoke(context);
        }

        private bool CheckRpcHeader(IHeaderDictionary headers)
        {
            if (!headers.ContainsKey("Cadmean-RPC-Version"))
                return false;
            
            var rpcVersionHeaderValue = headers["Cadmean-RPC-Version"][0];
            return rpcVersionHeaderValue == RpcServerConfiguration.SupportedCadmeanRpcVersion;
        }

        private async Task SendErrorOutput(HttpResponse response, RpcErrorCode code)
        {
            response.ContentType = "application/json";
            await response.WriteAsync(JsonConvert.SerializeObject(FunctionOutput.WithError(code)));
        }
    }
}