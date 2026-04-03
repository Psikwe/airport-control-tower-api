using AirportControlTower.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Domain.Entities
{
    public class Aircraft
    {
        public string CallSign { get; set; } = default!;
        public string Type { get; set; } = default!; // AIRLINER | PRIVATE

        public AircraftState State { get; set; } = AircraftState.PARKED;

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Altitude { get; set; }
        public int Heading { get; set; }
    }
}
