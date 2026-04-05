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
            if (_context.ParkingSpots.Any())
                return;

            // airliner spots
            for (int i = 0; i < _settings.AirlinerSpots; i++)
            {
                _context.ParkingSpots.Add(new ParkingSpot
                {
                    Type = AppConstants.AIRLINER,
                    IsOccupied = false
                });
            }

            // private spots
            for (int i = 0; i < _settings.PrivateSpots; i++)
            {
                _context.ParkingSpots.Add(new ParkingSpot
                {
                    Type = AppConstants.PRIVATE,
                    IsOccupied = false
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
