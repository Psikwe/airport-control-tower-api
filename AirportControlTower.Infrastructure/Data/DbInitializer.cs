using AirportControlTower.Domain.Entities;
using AirportControlTower.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task Seed(ApplicationDbContext context)
        {
            if (context.ParkingSpots.Any())
                return;

            for (int i = 0; i < 5; i++)
            {
                context.ParkingSpots.Add(new ParkingSpot
                {
                    Type = AppConstants.AIRLINER,
                    IsOccupied = false
                });
            }

            for (int i = 0; i < 10; i++)
            {
                context.ParkingSpots.Add(new ParkingSpot
                {
                    Type = AppConstants.PRIVATE,
                    IsOccupied = false
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
