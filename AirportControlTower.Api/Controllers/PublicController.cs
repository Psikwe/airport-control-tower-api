using AirportControlTower.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirportControlTower.Api.Controllers
{

    [ApiController]
    [Route("api/public/{callSign}")]
    public class PublicController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public PublicController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("weather")]
        public async Task<IActionResult> GetWeather(string callSign)
        {
            var weather = await _weatherService.GetLatestWeather();

            if (weather == null)
                return NotFound();

            return Ok(weather);
        }
    }
}
