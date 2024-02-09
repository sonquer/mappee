using Mappee.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Sample.WebApp.Models;

namespace Sample.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IMapper _mapper;

        public WeatherForecastController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecastDto> Get()
        {
            var weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });

            return weatherForecasts.Select(_mapper.Map<WeatherForecast, WeatherForecastDto>);
        }
    }
}