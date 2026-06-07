using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Configurations;

internal class SolarStationDailySnapshotConfiguration
    : IEntityTypeConfiguration<SolarStationDailySnapshot>
{
    public void Configure(EntityTypeBuilder<SolarStationDailySnapshot> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Day)
            .IsRequired();

        builder.Property(x => x.TotalKwh)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.HasOne<SolarStation>()
            .WithMany()
            .HasForeignKey(x => x.SolarStationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.SolarStationId, x.Day })
            .IsUnique();
    }
}