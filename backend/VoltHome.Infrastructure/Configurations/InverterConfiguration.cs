using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Configurations;

internal class InverterConfiguration : IEntityTypeConfiguration<Inverter>
{
    public void Configure(EntityTypeBuilder<Inverter> builder)
    {
        builder.Property(x => x.Power)
            .IsRequired();

        builder.Property(x => x.Efficiency)
            .IsRequired();

        builder.HasIndex(x => x.SolarStationId)
            .IsUnique(); 
    }
}
