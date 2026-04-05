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
    public class StateChangeLogConfiguration : IEntityTypeConfiguration<StateChangeLog>
    {
        public void Configure(EntityTypeBuilder<StateChangeLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CallSign)
                .IsRequired();

            builder.Property(x => x.Result)
                .IsRequired();

            builder.Property(x => x.Timestamp)
                .IsRequired();
        }
    }
}
