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
    private readonly IWeatherService _weatherService;
    private readonly ILogger<AircraftService> _logger;

    public AircraftService(IAircraftRepository repo, IWeatherService weatherService,
        ILogger<AircraftService> logger)
    {
        _repo = repo;
        _weatherService = weatherService;
        _logger = logger;
    }

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

    public async Task<(bool success, string? reason)> RequestStateChange(string callSign, string state)
    {
        if (!Enum.TryParse<AircraftState>(state, out var requestedState))
            return (false, "Invalid state");

        var aircraft = await _repo.GetByCallSign(callSign);

        if (aircraft == null)
            return (false, "Aircraft not found");

        if (!IsValidTransition(aircraft.State, requestedState))
        {
            var reason = $"Invalid transition: {aircraft.State} → {requestedState}";
            await _repo.LogStateChange(callSign, requestedState, AppConstants.REJECTED);
            return (false, reason);
        }

        var (allowed, reasonMsg) = await ValidateConstraints(aircraft, requestedState);

        if (!allowed)
        {
            await _repo.LogStateChange(callSign, requestedState, AppConstants.REJECTED);
            return (false, reasonMsg);
        }

        aircraft.State = requestedState;

        await _repo.Save(aircraft);

        await _repo.LogStateChange(callSign, requestedState, AppConstants.ACCEPTED);

        return (true, null);
    }

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

    private async Task<(bool allowed, string? reason)> ValidateConstraints(Aircraft aircraft, AircraftState requested)
    {
        var weather = await _weatherService.GetLatestWeather();

        if (weather != null)
        {
            if (requested == AircraftState.APPROACH)
            {
                if (weather.Wind.Speed > 10)
                {
                    var reason = "Landing denied: unsafe wind";
                    _logger.LogWarning("{Reason} for {CallSign}", reason, aircraft.CallSign);
                    return (false, reason);
                }

                if (weather.Visibility < 3000)
                {
                    var reason = "Landing denied: low visibility";
                    _logger.LogWarning("{Reason} for {CallSign}", reason, aircraft.CallSign);
                    return (false, reason);
                }
            }

            if (requested == AircraftState.TAKE_OFF)
            {
                if (weather.Wind.Speed > 20)
                {
                    var reason = "Takeoff denied: unsafe wind";
                    _logger.LogWarning("{Reason} for {CallSign}", reason, aircraft.CallSign);
                    return (false, reason);
                }
            }
        }

        if (requested == AircraftState.TAKE_OFF || requested == AircraftState.LANDED)
        {
            var runwayOccupied = await _repo.IsRunwayOccupied();
            if (runwayOccupied)
                return (false, "Runway is currently occupied");
        }

        if (requested == AircraftState.APPROACH)
        {
            var runwayOccupied = await _repo.IsRunwayOccupied();
            if (runwayOccupied)
                return (false, "Runway is occupied");

            var approachExists = await _repo.IsAnyAircraftOnApproach();
            if (approachExists)
                return (false, "Another aircraft is already on approach");

            var hasParking = await _repo.HasAvailableParking(aircraft.Type);
            if (!hasParking)
                return (false, "No parking available for this aircraft type");
        }

        return (true, null);
    }
}