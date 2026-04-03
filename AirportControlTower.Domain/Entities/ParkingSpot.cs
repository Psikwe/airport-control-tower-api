using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Domain.Entities
{
    public class ParkingSpot
    {
        public int Id { get; set; }

        public string Type { get; set; } = default!; // AIRLINER | PRIVATE

        public bool IsOccupied { get; set; }

        public string? AircraftCallSign { get; set; }
    }
}
