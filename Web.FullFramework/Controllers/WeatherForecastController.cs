using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Web.FullFramework.Controllers
{
    public class WeatherForecastController : ApiController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly string[] Cities = new[]
        {
            "Tacoma", "San Diego", "Lake Tahoe", "San Francisco", "Boston", "Kona", "Portland", "San Antonio"
        };

        [HttpGet, Route("weatherforecast", Name = "GetList")]
        public IHttpActionResult Get()
        {
            var rng = new Random();
            var model = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(1),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                City = Cities[index],
                Links = new List<Link>
                {
                    new Link { Rel = "list", Href = BuildAbsolutePath(Request, Url.Route("GetList", null)) },
                    new Link { Rel = "self", Href = BuildAbsolutePath(Request, Url.Route("Get", new { id = index + 1 })) }
                }
            }).ToArray();

            return Ok(model);
        }

        [HttpGet, Route("weatherforecast/{id}", Name = "Get")]
        public IHttpActionResult Get(int id)
        {
            var rng = new Random();
            var model = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                City = Cities[(id)],
                Links = new List<Link>
                {
                    new Link { Rel = "list", Href = BuildAbsolutePath(Request, Url.Route("GetList", null)) },
                    new Link { Rel = "self", Href = BuildAbsolutePath(Request, Url.Route("Get", new { id })) }
                }
            }).ToArray();

            return Ok(model);
        }

        private string BuildAbsolutePath(HttpRequestMessage request, string absolutePath)
        {
            var builder = new UriBuilder(request.RequestUri);
            builder.Path = absolutePath;

            return builder.Uri.AbsoluteUri;
        }
    }
}
