using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Web.ApiGateway
{
    public class LinkCorrectionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            var content = await response.Content.ReadAsStringAsync();

            if (response.Content.Headers.ContentType.MediaType == "application/json")
            {
                var builder = new UriBuilder(request.RequestUri);
                builder.Path = "/";
                string requestRootUri = builder.Uri.AbsoluteUri;
                content = Regex.Replace(content, requestRootUri, "https://127.0.0.1/v1/");

                var newResponse = new HttpResponseMessage(response.StatusCode)
                {
                    Content = new StringContent(content, encoding: System.Text.Encoding.UTF8, "application/json"),
                };

                return newResponse;
            }

            return response;
        }
    }

    public class LinkCorrectionMiddleware
    {
        private readonly RequestDelegate _next;

        public LinkCorrectionMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            await this._next(context);
        }
    }
}
