using AirportControlTower.Application.Abstractions;
using AirportControlTower.Domain.Entities;
using AirportControlTower.Domain.Enums;
using AirportControlTower.Infrastructure.Data;
using AirportControlTower.Shared.Configs;
using AirportControlTower.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Infrastructure.Repositories
{
    public class EfAircraftRepository : IAircraftRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly AirportSettings _settings;

        public EfAircraftRepository(ApplicationDbContext context, IOptions<AirportSettings> settings)
        {
            _context = context;
            _settings = settings.Value;
        }

        public async Task<Aircraft?> GetByCallSign(string callSign)
        {
            return await _context.Aircraft.FirstOrDefaultAsync(a => a.CallSign == callSign);
        }

        public async Task Save(Aircraft aircraft)
        {
            var existing = await _context.Aircraft
                .FirstOrDefaultAsync(a => a.CallSign == aircraft.CallSign);

            if (existing == null)
            {
                await _context.Aircraft.AddAsync(aircraft);
            }
            else
            {
                existing.State = aircraft.State;
                existing.Type = aircraft.Type;
                existing.Latitude = aircraft.Latitude;
                existing.Longitude = aircraft.Longitude;
                existing.Altitude = aircraft.Altitude;
                existing.Heading = aircraft.Heading;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsRunwayOccupied()
        {
            return await _context.Aircraft.AnyAsync(a =>
                a.State == AircraftState.TAKE_OFF ||
                a.State == AircraftState.LANDED);
        }

        public async Task<bool> IsAnyAircraftOnApproach()
        {
            return await _context.Aircraft.AnyAsync(a =>
                a.State == AircraftState.APPROACH);
        }

        public async Task<bool> HasAvailableParking(string type)
        {
            int occupied = await _context.Aircraft
                .CountAsync(a => a.State == AircraftState.PARKED && a.Type == type);

            int max = type == AppConstants.AIRLINER
                ? _settings.AirlinerSpots
                : _settings.PrivateSpots;

            return occupied < max;
        }

        public async Task LogStateChange(string callSign, AircraftState state, string result)
        {
            var log = new StateChangeLog
            {
                CallSign = callSign,
                RequestedState = state,
                Result = result,
                Timestamp = DateTime.UtcNow
            };

            await _context.StateChangeLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Aircraft>> GetAll()
        {
            return await _context.Aircraft.ToListAsync();
        }

        public async Task<List<StateChangeLog>> GetLogs()
        {
            return await _context.StateChangeLogs
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();
        }

        public async Task<List<StateChangeLog>> GetLast10Logs()
        {
            return await _context.StateChangeLogs
                .OrderByDescending(x => x.Timestamp)
                .Take(10)
                .ToListAsync();
        }

        public async Task<ParkingSpot?> GetFreeParkingSpot(string type)
        {
            return await _context.ParkingSpots
                .FirstOrDefaultAsync(p => !p.IsOccupied && p.Type == type);
        }

        public async Task AssignParking(string callSign, ParkingSpot spot)
        {
            spot.IsOccupied = true;
            spot.AircraftCallSign = callSign;

            await _context.SaveChangesAsync();
        }

        public async Task<Weather?> GetLatestWeather()
        {
            return await _context.Weather
                .OrderByDescending(x => x.LastUpdated)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ParkingSpot>> GetParkingSpots()
        {
            return await _context.ParkingSpots.ToListAsync();
        }
    }
}
