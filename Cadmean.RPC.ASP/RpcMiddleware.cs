using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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

            if (!FunctionPathParser.IsValidRpcPath(pathStr, rpcService.Configuration))
            {
                await next.Invoke(context);
                return;
            }

            if (!CheckRpcHeader(headers))
            {
                response.StatusCode = 418;
                await response.WriteAsync("Not supported cadRPC version");
                return;
            }
            
            var fName = FunctionPathParser.GetFunctionName(pathStr, rpcService.Configuration);
            var functionInfo = rpcService.GetCachedFunctionInfo(fName);

            if (functionInfo == null)
            {
                response.StatusCode = 404;
                await response.WriteAsync("Function not found");
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
            
            var rpcVersionHeaderValue = Convert.ToInt32(headers["Cadmean-RPC-Version"][0]);
            return rpcVersionHeaderValue == RpcServerConfiguration.SupportedCadmeanRpcVersion;
        }
    }
}