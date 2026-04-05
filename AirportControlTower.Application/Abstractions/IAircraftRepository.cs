using AirportControlTower.Domain.Entities;
using AirportControlTower.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Application.Abstractions
{
    public interface IAircraftRepository
    {
        Task<Aircraft?> GetByCallSign(string callSign);
        Task Save(Aircraft aircraft);
        Task<bool> IsRunwayOccupied();
        Task<bool> IsAnyAircraftOnApproach();
        Task<bool> HasAvailableParking(string type);
        Task LogStateChange(string callSign, AircraftState state, string result);
        Task<List<StateChangeLog>> GetLogs();
        Task<List<StateChangeLog>> GetLast10Logs();
        Task<ParkingSpot?> GetFreeParkingSpot(string type);
        Task AssignParking(string callSign, ParkingSpot spot);
        Task<List<Aircraft>> GetAll();
        Task<Weather?> GetLatestWeather();
        Task<List<ParkingSpot>> GetParkingSpots();
    }
}
