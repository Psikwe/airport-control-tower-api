using AirportControlTower.Application.Abstractions;
using AirportControlTower.Application.Services.Interfaces;
using AirportControlTower.Domain.Enums;
using AirportControlTower.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Application.Services
{
    public class GroundCrewService : IGroundCrewService
    {
        private readonly IAircraftRepository _repo;

        public GroundCrewService(IAircraftRepository repo)
        {
            _repo = repo;
        }

        public async Task ProcessAsync()
        {
            var aircrafts = await _repo.GetAll();

            var landedAircrafts = aircrafts
                .Where(a => a.State == AircraftState.LANDED)
                .ToList();

            foreach (var aircraft in landedAircrafts)
            {
                var spot = await _repo.GetFreeParkingSpot(aircraft.Type);

                if (spot == null)
                    continue;

                await _repo.AssignParking(aircraft.CallSign, spot);

                aircraft.State = AircraftState.PARKED;

                await _repo.Save(aircraft);

                await _repo.LogStateChange(
                    aircraft.CallSign,
                    AircraftState.PARKED,
                    AppConstants.ACCEPTED
                );
            }
        }
    }
}
