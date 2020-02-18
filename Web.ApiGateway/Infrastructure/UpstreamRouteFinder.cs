using System;
using System.Collections.Generic;
using System.Linq;
using Ocelot.Configuration;
using Ocelot.DownstreamRouteFinder.UrlMatcher;
using Ocelot.Responses;

namespace Web.ApiGateway.Infrastructure
{
    public interface IUpstreamRouteFinder
    {
        Response<UpstreamRoute> Get(string downstreamUrlPath, string downstreamQueryString, string httpMethod, IInternalConfiguration configuration, string downstreamHost);
    }

    public class UpstreamRouteFinder : IUpstreamRouteFinder
    {
        private readonly IUrlPathToUrlTemplateMatcher _urlMatcher;
        private readonly IPlaceholderNameAndValueFinder _placeholderNameAndValueFinder;

        public UpstreamRouteFinder(IUrlPathToUrlTemplateMatcher urlMatcher, IPlaceholderNameAndValueFinder urlPathPlaceholderNameAndValueFinder)
        {
            _urlMatcher = urlMatcher;
            _placeholderNameAndValueFinder = urlPathPlaceholderNameAndValueFinder;
        }

        public Response<UpstreamRoute> Get(string downstreamUrlPath, string downstreamQueryString, string httpMethod, IInternalConfiguration configuration, string downstreamHost)
        {
            var upstreamRoutes = new List<UpstreamRoute>();

            var applicableReRoutes = configuration.ReRoutes
                .Where(r => RouteIsApplicableToThisRequest(r, httpMethod, downstreamHost))
                .OrderByDescending(x => x.UpstreamTemplatePattern.Priority);

            foreach (var reRoute in applicableReRoutes)
            {
                var urlMatch = _urlMatcher.Match(downstreamUrlPath, downstreamQueryString, reRoute.DownstreamReRoute.First().DownstreamPathTemplate);

                if (urlMatch.Data.Match)
                {
                    upstreamRoutes.Add(GetPlaceholderNamesAndValues(downstreamUrlPath, downstreamQueryString, reRoute));
                }
            }

            if (upstreamRoutes.Any())
            {
                var notNullOption = upstreamRoutes.FirstOrDefault(x => !string.IsNullOrEmpty(x.ReRoute.UpstreamHost));
                var nullOption = upstreamRoutes.FirstOrDefault(x => string.IsNullOrEmpty(x.ReRoute.UpstreamHost));

                return notNullOption != null ? new OkResponse<UpstreamRoute>(notNullOption) : new OkResponse<UpstreamRoute>(nullOption);
            }

            return new ErrorResponse<UpstreamRoute>(new UnableToFindDownstreamRouteError(upstreamUrlPath, httpMethod));
        }

        private bool RouteIsApplicableToThisRequest(ReRoute reRoute, string httpMethod, string upstreamHost)
        {
            return (reRoute.UpstreamHttpMethod.Count == 0 || reRoute.UpstreamHttpMethod.Select(x => x.Method.ToLower()).Contains(httpMethod.ToLower())) &&
                   (string.IsNullOrEmpty(reRoute.UpstreamHost) || reRoute.UpstreamHost == upstreamHost);
        }

        private UpstreamRoute GetPlaceholderNamesAndValues(string path, string query, ReRoute reRoute)
        {
            var templatePlaceholderNameAndValues = _placeholderNameAndValueFinder.Find(path, query, reRoute.DownstreamReRoute.First().DownstreamPathTemplate.Value);

            return new UpstreamRoute(templatePlaceholderNameAndValues.Data, reRoute);
        }
    }

    public class UpstreamRoute
    {
        public UpstreamRoute(List<PlaceholderNameAndValue> templatePlaceholderNameAndValues, ReRoute reRoute)
        {
            TemplatePlaceholderNameAndValues = templatePlaceholderNameAndValues;
            ReRoute = reRoute;
        }

        public List<PlaceholderNameAndValue> TemplatePlaceholderNameAndValues { get; private set; }
        public ReRoute ReRoute { get; private set; }
    }
}
