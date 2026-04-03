using AirportControlTower.Application.Services.Interfaces;
using AirportControlTower.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Application.Services
{
    public class AircraftService : IAircraftService
    {
        public Task<bool> UpdateLocation(string callSign, LocationDto dto)
        {
            // TODO: implement
            return Task.FromResult(true);
        }

        public Task<bool> RequestStateChange(string callSign, string state)
        {
            // TODO: implement core logic here
            return Task.FromResult(true);
        }
    }
}
