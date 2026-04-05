using AirportControlTower.Application.Abstractions;
using AirportControlTower.Application.Services.Interfaces;
using AirportControlTower.Domain.Entities;
using AirportControlTower.Domain.Enums;
using AirportControlTower.Shared.Constants;
using AirportControlTower.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace AirportControlTower.Application.Services;

public class AircraftService : IAircraftService
{
    private readonly IAircraftRepository _repo;
    private readonly ILogger<AircraftService> _logger;

    public AircraftService(IAircraftRepository repo, ILogger<AircraftService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    // ---------------------------
    // UPDATE LOCATION
    // ---------------------------
    public async Task<bool> UpdateLocation(string callSign, LocationDto dto)
    {
        var aircraft = await _repo.GetByCallSign(callSign);

        if (aircraft == null)
        {
            aircraft = new Aircraft
            {
                CallSign = callSign,
                State = AircraftState.PARKED
            };
        }

        aircraft.Type = dto.Type;
        aircraft.Latitude = dto.Latitude;
        aircraft.Longitude = dto.Longitude;
        aircraft.Altitude = dto.Altitude;
        aircraft.Heading = dto.Heading;

        await _repo.Save(aircraft);
        _logger.LogInformation("Updating location for {CallSign}", callSign);
        return true;
    }

    // ---------------------------
    // REQUEST STATE CHANGE
    // ---------------------------
    public async Task<bool> RequestStateChange(string callSign, string state)
    {
        if (!Enum.TryParse<AircraftState>(state, out var requestedState))
            return false;

        var aircraft = await _repo.GetByCallSign(callSign);

        if (aircraft == null)
            return false;

        // 1. Validate transition
        if (!IsValidTransition(aircraft.State, requestedState))
        {
            await _repo.LogStateChange(callSign, requestedState, AppConstants.REJECTED);
            return false;
        }

        // 2. Enforce constraints
        var allowed = await ValidateConstraints(aircraft, requestedState);

        if (!allowed)
        {
            await _repo.LogStateChange(callSign, requestedState, AppConstants.REJECTED);
            return false;
        }

        // 3. Apply state change
        aircraft.State = requestedState;

        await _repo.Save(aircraft);

        await _repo.LogStateChange(callSign, requestedState, AppConstants.ACCEPTED);

        return true;
    }

    // ---------------------------
    // STATE MACHINE
    // ---------------------------
    private bool IsValidTransition(AircraftState current, AircraftState next)
    {
        return current switch
        {
            AircraftState.PARKED => next == AircraftState.TAKE_OFF,
            AircraftState.TAKE_OFF => next == AircraftState.AIRBORNE,
            AircraftState.AIRBORNE => next == AircraftState.APPROACH,
            AircraftState.APPROACH => next == AircraftState.LANDED || next == AircraftState.AIRBORNE,
            AircraftState.LANDED => false, // handled by ground crew only
            _ => false
        };
    }

    // ---------------------------
    // GLOBAL RULES
    // ---------------------------
    private async Task<bool> ValidateConstraints(Aircraft aircraft, AircraftState requested)
    {
        // RUNWAY RULE
        if (requested == AircraftState.TAKE_OFF || requested == AircraftState.LANDED)
        {
            var runwayOccupied = await _repo.IsRunwayOccupied();
            if (runwayOccupied) return false;
        }

        // APPROACH RULE
        if (requested == AircraftState.APPROACH)
        {
            var runwayOccupied = await _repo.IsRunwayOccupied();
            if (runwayOccupied) return false;

            var approachExists = await _repo.IsAnyAircraftOnApproach();
            if (approachExists) return false;

            var hasParking = await _repo.HasAvailableParking(aircraft.Type);
            if (!hasParking) return false;
        }

        return true;
    }
}