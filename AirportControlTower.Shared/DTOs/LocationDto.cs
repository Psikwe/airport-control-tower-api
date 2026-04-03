using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Shared.DTOs
{
    public class LocationDto
    {
        public string Type { get; set; } = default!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Altitude { get; set; }
        public int Heading { get; set; }
    }
}
