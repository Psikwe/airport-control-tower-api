using AirportControlTower.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Infrastructure.BackgroundServices
{
    public class GroundCrewWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GroundCrewWorker> _logger;

        public GroundCrewWorker(IServiceProvider serviceProvider, ILogger<GroundCrewWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("GroundCrewWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var service = scope.ServiceProvider.GetRequiredService<IGroundCrewService>();

                    await service.ProcessAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in GroundCrewWorker");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
