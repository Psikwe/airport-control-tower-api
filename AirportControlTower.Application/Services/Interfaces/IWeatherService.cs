using AirportControlTower.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Application.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherDto?> GetLatestWeather();
    }
}
