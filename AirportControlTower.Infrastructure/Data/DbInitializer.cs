using AirportControlTower.Domain.Entities;
using AirportControlTower.Shared.Configs;
using AirportControlTower.Shared.Constants;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Infrastructure.Data
{
    public class DbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly AirportSettings _settings;

        public DbInitializer(
            ApplicationDbContext context,
            IOptions<AirportSettings> settings)
        {
            _context = context;
            _settings = settings.Value;
        }

        public async Task SeedAsync()
        {
            if (!_context.ParkingSpots.Any())
            {
                for (int i = 0; i < _settings.AirlinerSpots; i++)
                {
                    _context.ParkingSpots.Add(new ParkingSpot
                    {
                        Type = AppConstants.AIRLINER,
                        IsOccupied = false
                    });
                }

                for (int i = 0; i < _settings.PrivateSpots; i++)
                {
                    _context.ParkingSpots.Add(new ParkingSpot
                    {
                        Type = AppConstants.PRIVATE,
                        IsOccupied = false
                    });
                }

                Console.WriteLine("Parking seeded");
            }

            if (!_context.Weather.Any())
            {
                Console.WriteLine("Seeding Weather...");

                _context.Weather.AddRange(
                    new Weather
                    {
                        Description = "clear sky",
                        Temperature = 25,
                        Visibility = 10000,
                        WindSpeed = 3,
                        WindDeg = 180,
                        LastUpdated = DateTime.UtcNow.AddMinutes(-10)
                    },
                    new Weather
                    {
                        Description = "heavy rain",
                        Temperature = 20,
                        Visibility = 2000,
                        WindSpeed = 15,
                        WindDeg = 250,
                        LastUpdated = DateTime.UtcNow
                    }
                );

                await _context.SaveChangesAsync();

                Console.WriteLine("Weather seeded count: " + _context.Weather.Count());
            }
            else
            {
                Console.WriteLine("Weather already exists, skipping...");
            }
        }
    }
}
