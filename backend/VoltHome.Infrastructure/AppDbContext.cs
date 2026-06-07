using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure.Configurations;

namespace VoltHome.Infrastructure;

public class AppDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<SolarStation> SolarStations { get; set; }
    public DbSet<PanelGroup> PanelGroups { get; set; }
    public DbSet<Inverter> Inverters { get; set; }
    public DbSet<SolarRegion> SolarRegions { get; set; }
    public DbSet<SolarMonthlyCoefficient> SolarMonthlyCoefficients { get; set; }
    public DbSet<SolarHourlyCoefficient> SolarHourlyCoefficients { get; set; }
    public DbSet<SolarStationEnergySnapshot> SolarStationEnergySnapshots { get; set; }
    public DbSet<SolarStationDailySnapshot> SolarStationDailySnapshots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new SolarStationConfiguration());
        modelBuilder.ApplyConfiguration(new PanelGroupConfiguration());
        modelBuilder.ApplyConfiguration(new InverterConfiguration());
        modelBuilder.ApplyConfiguration(new SolarRegionConfiguration());

        modelBuilder.ApplyConfiguration(new SolarMonthlyCoefficientConfiguration());
        modelBuilder.ApplyConfiguration(new SolarHourlyCoefficientConfiguration());

        modelBuilder.ApplyConfiguration(new SolarStationEnergySnapshotConfiguration());
        modelBuilder.ApplyConfiguration(new SolarStationDailySnapshotConfiguration());
    }
}