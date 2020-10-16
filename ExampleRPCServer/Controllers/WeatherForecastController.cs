using System;
using System.Collections.Generic;
using System.Linq;
using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Mvc;

namespace ExampleRPCServer.Controllers
{
    [ApiController]
    [Route("api/rpc/weatherForecast.get")]
    public class WeatherForecastController : FunctionController
    {
        private static readonly string[] Summaries = 
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public IEnumerable<WeatherForecast> OnCall()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}