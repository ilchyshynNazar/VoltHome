using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Seeders;

public static class SolarHourlyCoefficientSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Set<SolarHourlyCoefficient>().AnyAsync())
            return;

        var data = new List<SolarHourlyCoefficient>
        {
            Create(0, 0.00),
            Create(1, 0.00),
            Create(2, 0.00),
            Create(3, 0.00),
            Create(4, 0.00),
            Create(5, 0.00),
            Create(6, 0.03),
            Create(7, 0.08),
            Create(8, 0.18),
            Create(9, 0.30),
            Create(10, 0.42),
            Create(11, 0.55),
            Create(12, 0.62),
            Create(13, 0.65),
            Create(14, 0.60),
            Create(15, 0.48),
            Create(16, 0.35),
            Create(17, 0.22),
            Create(18, 0.12),
            Create(19, 0.05),
            Create(20, 0.01),
            Create(21, 0.00),
            Create(22, 0.00),
            Create(23, 0.00),
        };

        await context.AddRangeAsync(data);
        await context.SaveChangesAsync();
    }

    private static SolarHourlyCoefficient Create(int hour, double coeff)
    {
        return new SolarHourlyCoefficient
        {
            Id = Guid.NewGuid(),
            Hour = hour,
            Coefficient = coeff
        };
    }
}
