using AirportControlTower.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AirportControlTower.Application.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IConfiguration _config;

        public AuthorizationService(IConfiguration config)
        {
            _config = config;
        }

        public bool Validate(string callSign, string? key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            var expected = _config[$"AircraftKeys:{callSign}"];

            return expected == key;
        }
    }
}
