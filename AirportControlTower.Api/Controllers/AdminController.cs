using AirportControlTower.Application.Services.Interfaces;
using AirportControlTower.Domain.Entities;
using AirportControlTower.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirportControlTower.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;
        private readonly ApplicationDbContext _context;

        public AdminController(IAdminService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet("aircraft")]
        public async Task<IActionResult> GetAllAircraft()
        {
            var result = await _service.GetAllAircraft();
            return Ok(result);
        }

        [HttpGet("aircraft/{callSign}")]
        public async Task<IActionResult> GetAircraft(string callSign)
        {
            var result = await _service.GetAircraft(callSign);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs()
        {
            var result = await _service.GetLogs();
            return Ok(result);
        }

        [HttpGet("logs/latest")]
        public async Task<IActionResult> GetLast10Logs()
        {
            var result = await _service.GetLast10Logs();
            return Ok(result);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var data = await _service.GetDashboard();
            return Ok(data);
        }

        [HttpGet("parking")]
        public async Task<IActionResult> GetParking()
        {
            var result = await _service.GetParkingOverview();
            return Ok(result);
        }

        [HttpPost("weather/{type}")]
        public async Task<IActionResult> SwitchWeather(string type)
        {
            var now = DateTime.UtcNow;

            var weather = type.ToLower() switch
            {
                "clear" => new Weather
                {
                    Description = "clear sky",
                    Temperature = 25,
                    Visibility = 10000,
                    WindSpeed = 3,
                    WindDeg = 180,
                    LastUpdated = now
                },
                "rain" => new Weather
                {
                    Description = "heavy rain",
                    Temperature = 20,
                    Visibility = 2000,
                    WindSpeed = 15,
                    WindDeg = 250,
                    LastUpdated = now
                },
                _ => null
            };

            if (weather == null)
                return BadRequest("Invalid weather type");

            _context.Weather.RemoveRange(_context.Weather);
            await _context.Weather.AddAsync(weather);
            await _context.SaveChangesAsync();

            return Ok($"Weather switched to {type}");
        }
    }
}
