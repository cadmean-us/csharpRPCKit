using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Cadmean.RPC.ASP
{
    public static class Extensions
    {
        public static void AddRpc(this IServiceCollection services)
        {
            services.AddSingleton(new RpcService());
        }

        public static void UseRpc(this IApplicationBuilder app)
        {
            app.UseMiddleware<RpcMiddleware>();
        }
    }
}