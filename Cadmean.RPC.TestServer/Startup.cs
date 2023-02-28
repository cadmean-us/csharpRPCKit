using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cadmean.CoreKit.Authentication;
using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cadmean.RPC.TestServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRpc(rpcConfiguration =>
            {
                rpcConfiguration.UseAuthorization(token => 
                    new JwtToken(token).Validate(JwtAuthorizationOptions.Default));
                rpcConfiguration.DebugMode = true;
                rpcConfiguration.ExceptionHandler = Console.WriteLine;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();
            app.UseRpc();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}