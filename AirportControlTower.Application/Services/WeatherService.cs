using AirportControlTower.Application.Abstractions;
using AirportControlTower.Application.Services.Interfaces;
using AirportControlTower.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IAircraftRepository _repo;

        public WeatherService(IAircraftRepository repo)
        {
            _repo = repo;
        }

        public async Task<WeatherDto?> GetLatestWeather()
        {
            var weather = await _repo.GetLatestWeather();

            if (weather == null)
                return null;

            return new WeatherDto
            {
                Description = weather.Description,
                Temperature = weather.Temperature,
                Visibility = weather.Visibility,
                LastUpdate = weather.LastUpdated,
                Wind = new WindDto
                {
                    Speed = weather.WindSpeed,
                    Deg = weather.WindDeg
                }
            };
        }
    }
}
