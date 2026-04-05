using AirportControlTower.Domain.Entities;
using AirportControlTower.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirportControlTower.Infrastructure.ExternalServices
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;

        public WeatherService(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task FetchAndStoreWeather()
        {
            var url = "https://api.openweathermap.org/data/2.5/weather?q=Belgrade&appid=1a1f91e2241e9056cf2dd4f9cf66e8da&units=metric";

            var response = await _httpClient.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);

            var root = doc.RootElement;

            var weather = new Weather
            {
                Description = root.GetProperty("weather")[0].GetProperty("description").GetString()!,
                Temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
                Visibility = root.GetProperty("visibility").GetInt32(),
                WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                WindDeg = root.GetProperty("wind").GetProperty("deg").GetInt32(),
                LastUpdated = DateTime.UtcNow
            };

            _context.Weather.RemoveRange(_context.Weather);
            await _context.Weather.AddAsync(weather);
            await _context.SaveChangesAsync();
        }
    }
}
