using AirportControlTower.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Domain.Entities
{
    public class StateChangeLog
    {
        public int Id { get; set; }

        public string CallSign { get; set; } = default!;

        public AircraftState RequestedState { get; set; }

        public string Result { get; set; } = default!; // ACCEPTED / REJECTED

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

//continue at 1. AppConstants