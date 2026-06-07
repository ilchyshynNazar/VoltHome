using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Infrastructure.Seeders;

namespace VoltHome.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await SolarRegionSeeder.SeedAsync(context);
        await SolarHourlyCoefficientSeeder.SeedAsync(context);
        await SolarMonthlyCoefficientSeeder.SeedAsync(context);
    }
}
