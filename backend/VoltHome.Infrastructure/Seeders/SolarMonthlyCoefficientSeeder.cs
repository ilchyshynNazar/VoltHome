using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Seeders;

public static class SolarMonthlyCoefficientSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Set<SolarMonthlyCoefficient>().AnyAsync())
            return;

        var data = new List<SolarMonthlyCoefficient>
        {
            Create(1, 0.18), 
            Create(2, 0.30), 
            Create(3, 0.55), 
            Create(4, 0.75),
            Create(5, 0.90), 
            Create(6, 0.98), 
            Create(7, 1.00), 
            Create(8, 0.88), 
            Create(9, 0.65), 
            Create(10, 0.42),         
            Create(11, 0.22),
            Create(12, 0.15)  
        };

        await context.AddRangeAsync(data);
        await context.SaveChangesAsync();
    }

    private static SolarMonthlyCoefficient Create(int month, double coeff)
    {
        return new SolarMonthlyCoefficient
        {
            Id = Guid.NewGuid(),
            Month = month,
            Coefficient = coeff
        };
    }
}
