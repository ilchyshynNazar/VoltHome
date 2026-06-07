using Microsoft.EntityFrameworkCore;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure;
using VoltHome.Infrastructure.Configurations.Interfaces;
using VoltHome.Services.Interfaces;

public class SolarDailyAggregationService : ISolarDailyAggregationService
{
    private readonly ISolarSnapshotRepository _snapRepo;
    private readonly AppDbContext _context;

    public SolarDailyAggregationService(
        ISolarSnapshotRepository snapRepo,
        AppDbContext context)
    {
        _snapRepo = snapRepo;
        _context = context;
    }

    public async Task CalculateDayAsync(DateTime day, CancellationToken ct)
    {
        var start = day.Date.ToUniversalTime();
        var end = start.AddDays(1);

        var stations = await _context.SolarStations
            .Select(x => x.Id)
            .ToListAsync(ct);

        foreach (var stationId in stations)
        {
            var snapshots = await _snapRepo.GetByRangeAsync(
                stationId,
                start,
                end,
                ct);

            var total = snapshots.Sum(x => x.GeneratedKwh);

            var existing = await _context.SolarStationDailySnapshots
                .FirstOrDefaultAsync(x =>
                    x.SolarStationId == stationId &&
                    x.Day == start, ct);

            if (existing != null)
            {
                existing.TotalKwh = total;
            }
            else
            {
                await _context.SolarStationDailySnapshots.AddAsync(
                    new SolarStationDailySnapshot
                    {
                        Id = Guid.NewGuid(),
                        SolarStationId = stationId,
                        Day = start,
                        TotalKwh = total
                    }, ct);
            }
        }

        await _context.SaveChangesAsync(ct);
    }
}