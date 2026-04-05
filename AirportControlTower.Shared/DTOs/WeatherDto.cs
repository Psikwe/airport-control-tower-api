using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Shared.DTOs
{
    public class WeatherDto
    {
        public string Description { get; set; } = default!;
        public double Temperature { get; set; }
        public int Visibility { get; set; }
        public WindDto Wind { get; set; } = new();
        public DateTime LastUpdate { get; set; }
    }

    public class WindDto
    {
        public double Speed { get; set; }
        public int Deg { get; set; }
    }
}
