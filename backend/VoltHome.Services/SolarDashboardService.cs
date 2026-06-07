using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoltHome.Contracts.Responses.SolarStation;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure;
//using VoltHome.Infrastructure.Interfaces;
using VoltHome.Services.Interfaces;

namespace VoltHome.Services;

public class SolarDashboardService : ISolarDashboardService
{
    private readonly AppDbContext _context;
    private readonly ISolarEnergyCalculationService _calc;

    public SolarDashboardService(
        AppDbContext context,
        ISolarEnergyCalculationService calc)
    {
        _context = context;
        _calc = calc;
    }

    public async Task<SolarDashboardDto> GetDashboardAsync(
        Guid stationId,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var hour = NormalizeHour(now);

        var station = await _context.SolarStations
            .AsNoTracking()
            .Include(x => x.PanelGroups)
            .Include(x => x.Inverter)
            .FirstOrDefaultAsync(x => x.Id == stationId, ct);

        if (station?.Inverter == null || station.PanelGroups == null)
            throw new Exception("Station not found");

        var currentHourKwh =
            await _calc.EstimateStationHourKwhAsync(
                stationId,
                hour,
                ct);

        double todayKwh = 0;

        for (var h = 0; h <= hour.Hour; h++)
        {
            var calcHour = new DateTime(
                hour.Year,
                hour.Month,
                hour.Day,
                h, 0, 0,
                DateTimeKind.Utc);

            todayKwh += await _calc.EstimateStationHourKwhAsync(
                stationId,
                calcHour,
                ct);
        }

        double fullDayForecastKwh = 0;

        for (var h = 0; h < 24; h++)
        {
            var calcHour = new DateTime(
                hour.Year,
                hour.Month,
                hour.Day,
                h, 0, 0,
                DateTimeKind.Utc);

            fullDayForecastKwh +=
                await _calc.EstimateStationHourKwhAsync(
                    stationId,
                    calcHour,
                    ct);
        }

        var nextHour = hour.AddHours(1);

        var nextHourForecast =
            await _calc.EstimateStationHourKwhAsync(
                stationId,
                nextHour,
                ct);

        var peakDcKw = station.PanelGroups
            .Sum(pg => pg.PanelCount * pg.PowerPerPanel) / 1000.0;

        var peakAcKw = Math.Min(peakDcKw, station.Inverter.Power);

        var hourGaugeMax = Math.Max(
            Math.Max(0.35, peakAcKw * 0.55),
            currentHourKwh * 1.25);

        var dayGaugeMax = Math.Max(
            Math.Max(2.0, peakAcKw * 7.0),
            todayKwh * 1.12);

        var forecastGaugeMax = Math.Max(
            nextHourForecast * 1.3,
            peakAcKw);

        return new SolarDashboardDto
        {
            CurrentHourKwh = currentHourKwh,
            TodayKwh = todayKwh,

            ForecastNextHourKwh = nextHourForecast,
            ForecastTodayRemainingKwh = fullDayForecastKwh,

            HourGaugeMax = hourGaugeMax,
            DayGaugeMax = dayGaugeMax,
            ForecastGaugeMax = forecastGaugeMax
        };
    }

    private static DateTime NormalizeHour(DateTime dt)
        => new DateTime(
            dt.Year,
            dt.Month,
            dt.Day,
            dt.Hour,
            0,
            0,
            DateTimeKind.Utc);
}