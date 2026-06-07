using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Configurations;

internal class SolarStationConfiguration : IEntityTypeConfiguration<SolarStation>
{
    public void Configure(EntityTypeBuilder<SolarStation> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.SolarRegion)
            .WithMany()
            .HasForeignKey(x => x.SolarRegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.PanelGroups)
            .WithOne(x => x.SolarStation)
            .HasForeignKey(x => x.SolarStationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Inverter)
            .WithOne(x => x.SolarStation)
            .HasForeignKey<Inverter>(x => x.SolarStationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<SolarStationEnergySnapshot>()
            .WithOne()
            .HasForeignKey(x => x.SolarStationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
