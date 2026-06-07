using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Configurations;

internal class SolarStationEnergySnapshotConfiguration
    : IEntityTypeConfiguration<SolarStationEnergySnapshot>
{
    public void Configure(EntityTypeBuilder<SolarStationEnergySnapshot> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.HourStartUtc)
            .IsRequired();

        builder.Property(x => x.GeneratedKwh)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(x => x.SolarCoefficient)
            .IsRequired()
            .HasPrecision(5, 3);

        builder.HasIndex(x => new { x.SolarStationId, x.HourStartUtc })
            .IsUnique();
    }
}