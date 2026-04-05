using AirportControlTower.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirportControlTower.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Aircraft> Aircraft => Set<Aircraft>();
        public DbSet<ParkingSpot> ParkingSpots => Set<ParkingSpot>();
        public DbSet<StateChangeLog> StateChangeLogs => Set<StateChangeLog>();
        public DbSet<Weather> Weather => Set<Weather>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
