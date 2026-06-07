using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Configurations;

internal class PanelGroupConfiguration : IEntityTypeConfiguration<PanelGroup>
{
    public void Configure(EntityTypeBuilder<PanelGroup> builder)
    {
        builder.Property(x => x.PanelCount)
            .IsRequired();

        builder.Property(x => x.PowerPerPanel)
            .IsRequired();

        builder.Property(x => x.TiltAngle)
            .IsRequired();

        builder.Property(x => x.Azimuth)
            .IsRequired()
            .HasConversion<int>();
    }
}
