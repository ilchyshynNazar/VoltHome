using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Services.Interfaces;

namespace VoltHome.Services.BackgroundServices;

public class SolarDailyBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SolarDailyBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var kyivZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kyivZone);

            var nextRun = now.Date.AddDays(1).AddMinutes(5);
            var delay = nextRun - now;

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }

            using var scope = _scopeFactory.CreateScope();

            var service = scope.ServiceProvider
                .GetRequiredService<ISolarDailyAggregationService>();

            var day = now.Date;

            await service.CalculateDayAsync(day, stoppingToken);
        }
    }
}