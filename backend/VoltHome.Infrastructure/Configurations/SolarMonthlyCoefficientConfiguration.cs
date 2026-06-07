using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Configurations;

internal class SolarMonthlyCoefficientConfiguration
    : IEntityTypeConfiguration<SolarMonthlyCoefficient>
{
    public void Configure(EntityTypeBuilder<SolarMonthlyCoefficient> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Month)
            .IsRequired();

        builder.Property(x => x.Coefficient)
            .IsRequired()
            .HasPrecision(5, 3);

        builder.HasIndex(x => x.Month)
            .IsUnique();
    }
}

