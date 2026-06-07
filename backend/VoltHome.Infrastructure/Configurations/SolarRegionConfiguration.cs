using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Configurations;

internal class SolarRegionConfiguration : IEntityTypeConfiguration<SolarRegion>
{
    public void Configure(EntityTypeBuilder<SolarRegion> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IrradiationKwhPerM2Year)
            .IsRequired();

        builder.Property(x => x.GenerationPerKwYear)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}
