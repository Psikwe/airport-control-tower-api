using AirportControlTower.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportControlTower.Infrastructure.Configurations
{
    public class AircraftConfiguration : IEntityTypeConfiguration<Aircraft>
    {
        public void Configure(EntityTypeBuilder<Aircraft> builder)
        {
            builder.HasKey(x => x.CallSign);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.State)
                .IsRequired();

            builder.Property(x => x.Latitude).IsRequired();
            builder.Property(x => x.Longitude).IsRequired();
            builder.Property(x => x.Altitude).IsRequired();
            builder.Property(x => x.Heading).IsRequired();
        }
    }
}
