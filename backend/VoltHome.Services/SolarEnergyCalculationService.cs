using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure;
using VoltHome.Services.Interfaces;

namespace VoltHome.Services;

public class SolarEnergyCalculationService : ISolarEnergyCalculationService
{
    private readonly AppDbContext _context;

    public SolarEnergyCalculationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CalculateHourAsync(DateTime utcHour, CancellationToken ct)
    {
        var hourStart = NormalizeHour(utcHour);

        var stations = await _context.SolarStations
            .Include(x => x.PanelGroups)
            .Include(x => x.Inverter)
            .Include(x => x.SolarRegion)
            .ToListAsync(ct);

        var hourCoeff = await _context.SolarHourlyCoefficients
            .FirstAsync(x => x.Hour == hourStart.Hour, ct);

        var monthCoeff = await _context.SolarMonthlyCoefficients
            .FirstAsync(x => x.Month == hourStart.Month, ct);

        var existingSnapshots = await _context.SolarStationEnergySnapshots
            .Where(x => x.HourStartUtc == hourStart)
            .ToListAsync(ct);

        foreach (var station in stations)
        {
            if (existingSnapshots.Any(x => x.SolarStationId == station.Id))
                continue;

            var generatedKwh = ComputeGeneratedKwh(
                station,
                hourCoeff.Coefficient,
                monthCoeff.Coefficient);

            var solarCoefficient =
                hourCoeff.Coefficient *
                monthCoeff.Coefficient *
                station.Inverter!.Efficiency *
                CalculateAzimuthFactor(station.PanelGroups) *
                CalculateTiltFactor(station.PanelGroups);

            var snapshot = new SolarStationEnergySnapshot
            {
                Id = Guid.NewGuid(),
                SolarStationId = station.Id,
                HourStartUtc = hourStart,
                GeneratedKwh = generatedKwh,
                SolarCoefficient = solarCoefficient
            };

            await _context.SolarStationEnergySnapshots.AddAsync(snapshot, ct);
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<double> EstimateStationHourKwhAsync(Guid stationId, DateTime utcHour, CancellationToken ct)
    {
        var hourStart = NormalizeHour(utcHour);

        var station = await _context.SolarStations
            .Include(x => x.PanelGroups)
            .Include(x => x.Inverter)
            .FirstOrDefaultAsync(x => x.Id == stationId, ct);

        if (station?.Inverter == null ||
            station.PanelGroups == null ||
            station.PanelGroups.Count == 0)
            return 0;

        var hourCoeff = await _context.SolarHourlyCoefficients
            .FirstAsync(x => x.Hour == hourStart.Hour, ct);

        var monthCoeff = await _context.SolarMonthlyCoefficients
            .FirstAsync(x => x.Month == hourStart.Month, ct);

        return ComputeGeneratedKwh(station, hourCoeff.Coefficient, monthCoeff.Coefficient);
    }

    private static DateTime NormalizeHour(DateTime utcHour)
        => new DateTime(
            utcHour.Year,
            utcHour.Month,
            utcHour.Day,
            utcHour.Hour,
            0,
            0,
            DateTimeKind.Utc);

    private static double ComputeGeneratedKwh(
        SolarStation station,
        double hourCoefficient,
        double monthCoefficient)
    {
        var totalPanelPowerKw = station.PanelGroups!
            .Sum(pg => pg.PanelCount * pg.PowerPerPanel) / 1000.0;

        var inverterLimitedKw = Math.Min(
            totalPanelPowerKw,
            station.Inverter!.Power);

        var azimuthFactor = CalculateAzimuthFactor(station.PanelGroups);
        var tiltFactor = CalculateTiltFactor(station.PanelGroups);

        var solarCoefficient = hourCoefficient * monthCoefficient;

        var systemFactor =
            0.92 + ((station.Inverter.Efficiency - 0.9) * 0.2);

        var orientationFactor =
            0.9 + ((azimuthFactor - 0.7) * 0.15);

        var angleFactor =
            0.92 + ((tiltFactor - 0.7) * 0.1);

        var finalCoefficient =
            solarCoefficient *
            systemFactor *
            orientationFactor *
            angleFactor;

        return inverterLimitedKw * finalCoefficient;
    }

    private static double CalculateAzimuthFactor(ICollection<PanelGroup> groups)
    {
        var avg = 0.0;

        foreach (var group in groups)
        {
            var diff = Math.Abs((int)group.Azimuth);

            if (diff > 180)
                diff = 360 - diff;

            var penalty = diff / 180.0;

            var factor = 1.0 - (penalty * 0.15);

            avg += Math.Max(0.85, factor);
        }

        return avg / groups.Count;
    }

    private static double CalculateTiltFactor(ICollection<PanelGroup> groups)
    {
        const double optimal = 30.0;

        var avgTilt = groups.Average(x => x.TiltAngle);

        var diff = Math.Abs(avgTilt - optimal);

        var factor = 1.0 - (diff / 100.0);

        return Math.Max(0.88, factor);
    }
}