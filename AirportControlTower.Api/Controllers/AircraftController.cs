using AirportControlTower.Application.Services.Interfaces;
using AirportControlTower.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirportControlTower.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AircraftController : ControllerBase
    {
        private readonly IAircraftService _service;
        private readonly IAuthorizationService _auth;

        public AircraftController(IAircraftService service, IAuthorizationService auth)
        {
            _service = service;
            _auth = auth;
        }

        [HttpPut("location")]
        public async Task<IActionResult> UpdateLocation(
            string callSign,
            [FromBody] LocationDto dto)
        {
            var key = Request.Headers["X-Aircraft-Key"].FirstOrDefault();
            if (!_auth.Validate(callSign, key))
                return Unauthorized();
            var result = await _service.UpdateLocation(callSign, dto);

            return result ? NoContent() : BadRequest();
        }

        [HttpPost("intent")]
        public async Task<IActionResult> RequestIntent(
            string callSign,
            [FromBody] IntentDto dto)
        {
            var key = Request.Headers["X-Aircraft-Key"].FirstOrDefault();
            if (!_auth.Validate(callSign, key))
                return Unauthorized(); 
            
            if (string.IsNullOrWhiteSpace(dto.State))
                return BadRequest("State is required");

            var (success, reason) = await _service.RequestStateChange(callSign, dto.State);

            if (!success)
                return Conflict(new { message = reason });

            return NoContent();
        }
    }
}
