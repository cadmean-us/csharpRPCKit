using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Cadmean.RPC.ASP
{
    public static class Extensions
    {
        public static void AddRpc(this IServiceCollection services, RpcServerConfiguration configuration = null)
        {
            services.AddSingleton(new RpcService(configuration ?? new RpcServerConfiguration()));
        }
        
        public static void AddRpc(this IServiceCollection services, Action<RpcServerConfiguration> configure)
        {
            var configuration = new RpcServerConfiguration();
            configure(configuration);
            services.AddSingleton(new RpcService(configuration));
        }

        public static void UseRpc(this IApplicationBuilder app)
        {
            app.UseMiddleware<RpcMiddleware>();
        }
    }
}