using System;
using Ocelot.Configuration;
using Ocelot.DownstreamRouteFinder.UrlMatcher;
using Ocelot.Responses;
using Ocelot.Values;

namespace Web.ApiGateway.Infrastructure
{
    public interface IDownstreamUrlPathToTeUrlTemplateMatcher
    {
        public Response<UrlMatch> Match(string downstreamUrlPath, string downstreamQueryString, DownstreamReRoute reroute);
    }

    public class RegExDownstreamUrlMatcher : IDownstreamUrlPathToTeUrlTemplateMatcher
    {
        public Response<UrlMatch> Match(string downstreamUrlPath, string downstreamQueryString, DownstreamReRoute reroute)
        {
            
            return pathTemplate.Pattern.IsMatch($"{downstreamUrlPath}{downstreamQueryString}")
                ? new OkResponse<UrlMatch>(new UrlMatch(true))
                : new OkResponse<UrlMatch>(new UrlMatch(false));
        }
    }
}
