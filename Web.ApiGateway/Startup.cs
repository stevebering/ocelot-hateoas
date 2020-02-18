using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Web.ApiGateway
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var ocelotConfiguration = new OcelotPipelineConfiguration
            {
                PreQueryStringBuilderMiddleware = async (ctx, next) =>
                {
                    await next.Invoke();

                    var req = ctx.DownstreamRequest;
                    var res = ctx.DownstreamResponse;
                    var reroute = ctx.DownstreamReRoute;
                    var cfg = ctx.Configuration;

                    var content = await res.Content.ReadAsStringAsync();

                    if (res.Content.Headers.ContentType.MediaType == "application/json")
                    {
                        var builder = new UriBuilder(reroute.DownstreamPathTemplate.Value);
                        builder.Path = "/";
                        string requestRootUri = builder.Uri.AbsoluteUri;
                        content = Regex.Replace(content, requestRootUri, reroute.UpstreamPathTemplate.OriginalValue);

                        var newResponse = new HttpResponseMessage(response.StatusCode)
                        {
                            Content = new StringContent(content, encoding: System.Text.Encoding.UTF8, "application/json"),
                        };

                        ctx.DownstreamResponse = new DownstreamResponse(


                            )
                        res.Content = new StringC
                        
                        return newResponse;
                    }

                    return response;
                }
            };

            app.UseRouting();            
            app.UseOcelot(ocelotConfiguration).Wait();
        }
    }
}