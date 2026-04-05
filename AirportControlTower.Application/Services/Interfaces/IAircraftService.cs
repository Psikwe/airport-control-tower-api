using AirportControlTower.Domain.Entities;
using AirportControlTower.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Application.Services.Interfaces
{
    public interface IAircraftService
    {
        Task<bool> UpdateLocation(string callSign, LocationDto dto);
        Task<bool> RequestStateChange(string callSign, string state);
        Task<Weather?> GetLatestWeather();
    }
}
