using AirportControlTower.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirportControlTower.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;

        public AdminController(IAdminService service)
        {
            _service = service;
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
    }
}
