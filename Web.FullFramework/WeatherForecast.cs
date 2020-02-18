using System;
using System.Collections.Generic;

namespace Web.FullFramework
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }

        public string City { get; set; }

        public IEnumerable<Link> Links { get; set; }
    }

    public class Link
    {
        public string Rel { get; set; }
        public string Href { get; set; }
    }
}
