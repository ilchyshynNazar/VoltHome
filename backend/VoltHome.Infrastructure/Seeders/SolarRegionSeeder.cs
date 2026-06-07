using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Seeders;

public static class SolarRegionSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Set<SolarRegion>().AnyAsync())
            return;

        var regions = new List<SolarRegion>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Odeska",
                IrradiationKwhPerM2Year = 1500,
                GenerationPerKwYear = 1125
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Crimea",
                IrradiationKwhPerM2Year = 1500,
                GenerationPerKwYear = 1125
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Khersonska",
                IrradiationKwhPerM2Year = 1440,
                GenerationPerKwYear = 1080
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Mykolaivska",
                IrradiationKwhPerM2Year = 1400,
                GenerationPerKwYear = 1050
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Zaporizka",
                IrradiationKwhPerM2Year = 1360,
                GenerationPerKwYear = 1020
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Donetska",
                IrradiationKwhPerM2Year = 1340,
                GenerationPerKwYear = 1005
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Dnipropetrovska",
                IrradiationKwhPerM2Year = 1300,
                GenerationPerKwYear = 975
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Kirovohradska",
                IrradiationKwhPerM2Year = 1270,
                GenerationPerKwYear = 953
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Chernivetska",
                IrradiationKwhPerM2Year = 1250,
                GenerationPerKwYear = 938
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Vinnytska",
                IrradiationKwhPerM2Year = 1230,
                GenerationPerKwYear = 923
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Luhanska",
                IrradiationKwhPerM2Year = 1230,
                GenerationPerKwYear = 923
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Cherkaska",
                IrradiationKwhPerM2Year = 1200,
                GenerationPerKwYear = 900
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Zakarpatska",
                IrradiationKwhPerM2Year = 1200,
                GenerationPerKwYear = 900
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Poltavska",
                IrradiationKwhPerM2Year = 1170,
                GenerationPerKwYear = 878
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "IvanoFrankivska",
                IrradiationKwhPerM2Year = 1150,
                GenerationPerKwYear = 863
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Kharkivska",
                IrradiationKwhPerM2Year = 1140,
                GenerationPerKwYear = 855
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Khmelnytska",
                IrradiationKwhPerM2Year = 1140,
                GenerationPerKwYear = 855
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Ternopilska",
                IrradiationKwhPerM2Year = 1115,
                GenerationPerKwYear = 836
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Kyivska",
                IrradiationKwhPerM2Year = 1100,
                GenerationPerKwYear = 825
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Lvivska",
                IrradiationKwhPerM2Year = 1085,
                GenerationPerKwYear = 814
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Zhytomyrska",
                IrradiationKwhPerM2Year = 1085,
                GenerationPerKwYear = 814
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Volynska",
                IrradiationKwhPerM2Year = 1060,
                GenerationPerKwYear = 795
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Rivnenska",
                IrradiationKwhPerM2Year = 1060,
                GenerationPerKwYear = 795
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Sumska",
                IrradiationKwhPerM2Year = 1040,
                GenerationPerKwYear = 780
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Chernihivska",
                IrradiationKwhPerM2Year = 1015,
                GenerationPerKwYear = 761
            }
        };

        await context.AddRangeAsync(regions);
        await context.SaveChangesAsync();
    }
}
