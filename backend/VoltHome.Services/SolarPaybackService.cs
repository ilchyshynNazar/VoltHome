using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoltHome.Contracts.Responses.SolarStation;
using VoltHome.Infrastructure;
using VoltHome.Services.Interfaces;

namespace VoltHome.Services;

public class SolarPaybackService : ISolarPaybackService
{
    private readonly AppDbContext _context;
    private readonly SolarEconomicsOptions _economics;

    public SolarPaybackService(
        AppDbContext context,
        IOptions<SolarEconomicsOptions> economics)
    {
        _context = context;
        _economics = economics.Value;
    }

    public async Task<SolarPaybackInsightDto> GetInsightsAsync(
        Guid stationId,
        Guid ownerUserId,
        CancellationToken ct)
    {
        var station = await _context.SolarStations
            .AsNoTracking()
            .Include(x => x.PanelGroups)
            .Include(x => x.Inverter)
            .Include(x => x.SolarRegion)
            .FirstOrDefaultAsync(x => x.Id == stationId && x.OwnerId == ownerUserId, ct);

        if (station?.Inverter == null || station.SolarRegion == null)
            throw new Exception("Station not found");

        var peakDcKw = station.PanelGroups.Sum(pg => pg.PanelCount * pg.PowerPerPanel) / 1000.0;
        var peakAcKw = Math.Min(peakDcKw, station.Inverter.Power);

        var annualKwh = station.SolarRegion.GenerationPerKwYear * peakAcKw;

        var cost = peakAcKw * _economics.CostPerKwInstalledUah;
        var tariff = _economics.GreenTariffUahPerKwh;
        var revenue = annualKwh * tariff;
        var payback = revenue > 0 ? cost / revenue : 0;

        var months = await _context.SolarMonthlyCoefficients
            .AsNoTracking()
            .OrderBy(x => x.Month)
            .ToListAsync(ct);

        var maxCoeff = months.Count > 0 ? months.Max(x => x.Coefficient) : 1.0;

        var profile = months.Select(m => new MonthlyIrradiationPointDto
        {
            Month = m.Month,
            RelativeCoefficient = m.Coefficient,
            PercentOfPeak = maxCoeff > 0 ? Math.Round(100.0 * m.Coefficient / maxCoeff, 1) : 0
        }).ToList();

        return new SolarPaybackInsightDto
        {
            PeakDcKw = Math.Round(peakDcKw, 3),
            PeakAcKw = Math.Round(peakAcKw, 3),
            EstimatedAnnualKwh = Math.Round(annualKwh, 1),
            EstimatedSystemCostUah = Math.Round(cost, 0),
            AssumedGreenTariffUahPerKwh = tariff,
            EstimatedAnnualRevenueUah = Math.Round(revenue, 0),
            RoughPaybackYears = Math.Round(payback, 2),
            MonthlyProfile = profile,
            Disclaimer = _economics.Disclaimer
        };
    }
}
