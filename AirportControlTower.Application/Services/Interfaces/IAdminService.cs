using AirportControlTower.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Application.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<StateChangeLog>> GetLogs();
        Task<List<StateChangeLog>> GetLast10Logs();
        Task<List<Aircraft>> GetAllAircraft();
        Task<Aircraft?> GetAircraft(string callSign);
        Task<object> GetDashboard();
    }
}
