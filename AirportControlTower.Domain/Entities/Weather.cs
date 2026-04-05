using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Domain.Entities
{
    public class Weather
    {
        public int Id { get; set; }
        public string Description { get; set; } = default!;
        public double Temperature { get; set; }
        public int Visibility { get; set; }
        public double WindSpeed { get; set; }
        public int WindDeg { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
