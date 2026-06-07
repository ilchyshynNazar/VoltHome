using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Configurations;

internal class SolarHourlyCoefficientConfiguration
    : IEntityTypeConfiguration<SolarHourlyCoefficient>
{
    public void Configure(EntityTypeBuilder<SolarHourlyCoefficient> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Hour)
            .IsRequired();

        builder.Property(x => x.Coefficient)
            .IsRequired()
            .HasPrecision(5, 3);

        builder.HasIndex(x => x.Hour)
            .IsUnique(); 
    }
}
