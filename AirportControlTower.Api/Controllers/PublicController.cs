using AirportControlTower.Application.Services.Interfaces;
using AirportControlTower.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirportControlTower.Api.Controllers
{

    [ApiController]
    [Route("api/public")]
    public class PublicController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public PublicController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("{callSign}/weather")]
        public async Task<IActionResult> GetWeather([FromRoute] string callSign)
        {
            var weather = await _weatherService.GetLatestWeather();

            if (weather == null)
            {
                return Ok(new WeatherDto
                {
                    Description = "No data",
                    Temperature = 0,
                    Visibility = 0,
                    LastUpdate = DateTime.UtcNow
                });
            }

            return Ok(weather);
        }
    }
}
