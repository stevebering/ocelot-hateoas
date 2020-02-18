using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.CoreFramework.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly string[] Cities = new[]
        {
            "Tacoma", "San Diego", "Lake Tahoe", "San Francisco", "Boston", "Kona", "Portland", "San Antonio"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            var path = Request.Path;

            var rng = new Random();
            var model = Enumerable.Range(0, 7).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(1),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                City = Cities[index],
                Links = new List<Link>
                {
                    new Link { Rel = "list", Href = Url.Action("GetList", "WeatherForecast", null, protocol: Request.Scheme) },
                    new Link { Rel = "self", Href = Url.Action("Get", "WeatherForecast", new { id = index + 1 }, protocol: Request.Scheme) }
                }
            }).ToArray();

            return Ok(model);
        }


        [HttpGet, Route("{id}")]
        public IActionResult Get(int id)
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
                    new Link { Rel = "list", Href = Url.Action("GetList", "WeatherForecast", null, protocol: Request.Scheme) },
                    new Link { Rel = "self", Href = Url.Action("Get", "WeatherForecast", new { id }, protocol: Request.Scheme) }
                }
            }).ToArray();

            return Ok(model);
        }
    }
}