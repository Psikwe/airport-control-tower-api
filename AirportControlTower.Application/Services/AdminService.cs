using AirportControlTower.Application.Abstractions;
using AirportControlTower.Application.Services.Interfaces;
using AirportControlTower.Domain.Entities;

namespace AirportControlTower.Application.Services;

public class AdminService : IAdminService
{
    private readonly IAircraftRepository _repo;

    public AdminService(
        IAircraftRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Aircraft>> GetAllAircraft()
    {
        return await _repo.GetAll();
    }

    public async Task<Aircraft?> GetAircraft(string callSign)
    {
        return await _repo.GetByCallSign(callSign);
    }

    public async Task<List<StateChangeLog>> GetLogs()
    {
        return await _repo.GetLogs();
    }

    public async Task<List<StateChangeLog>> GetLast10Logs()
    {
        return await _repo.GetLast10Logs();
    }

    public async Task<object> GetDashboard()
    {
        var aircraft = await _repo.GetAll();
        var logs = await _repo.GetLast10Logs();
        var weather = await _repo.GetLatestWeather();

        return new
        {
            aircraftCount = aircraft.Count,
            lastLogs = logs,
            weather
        };
    }
}