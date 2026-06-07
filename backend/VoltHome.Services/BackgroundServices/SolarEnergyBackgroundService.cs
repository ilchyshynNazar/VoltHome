using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Services.Interfaces;

namespace VoltHome.Services.BackgroundServices;

public class SolarEnergyBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SolarEnergyBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var kyivZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

        while (!stoppingToken.IsCancellationRequested)
        {
            var utcNow = DateTime.UtcNow;

            var kyivNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, kyivZone);

            var nextHour = new DateTime(
                kyivNow.Year,
                kyivNow.Month,
                kyivNow.Day,
                kyivNow.Hour,
                0,
                0).AddHours(1);

            var delay = nextHour - kyivNow;

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }

            using var scope = _scopeFactory.CreateScope();

            var hourService = scope.ServiceProvider
                .GetRequiredService<ISolarEnergyCalculationService>();

            var dailyService = scope.ServiceProvider
                .GetRequiredService<ISolarDailyAggregationService>();

            var currentKyivHour = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                kyivZone);

            await hourService.CalculateHourAsync(currentKyivHour, stoppingToken);
        }
    }
}