using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure.Configurations.Interfaces;

namespace VoltHome.Infrastructure.Repositories;

public class SolarSnapshotRepository : ISolarSnapshotRepository
{
    private readonly AppDbContext _context;

    public SolarSnapshotRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid stationId, DateTime hour, CancellationToken ct)
    {
        var h = NormalizeHour(hour);

        return await _context.SolarStationEnergySnapshots
            .AnyAsync(x => x.SolarStationId == stationId && x.HourStartUtc == h, ct);
    }

    public async Task AddAsync(SolarStationEnergySnapshot snapshot, CancellationToken ct)
    {
        await _context.SolarStationEnergySnapshots.AddAsync(snapshot, ct);
    }

    public async Task<List<SolarStationEnergySnapshot>> GetByRangeAsync(Guid stationId, DateTime from, DateTime to, CancellationToken ct)
    {
        return await _context.SolarStationEnergySnapshots
            .Where(x => x.SolarStationId == stationId &&
                        x.HourStartUtc >= from &&
                        x.HourStartUtc < to)
            .ToListAsync(ct);
    }
    public async Task<SolarStationEnergySnapshot?> GetByHourAsync(
        Guid stationId,
        DateTime hour,
        CancellationToken ct)
    {
        var h = NormalizeHour(hour);

        return await _context.SolarStationEnergySnapshots
            .FirstOrDefaultAsync(x =>
                x.SolarStationId == stationId &&
                x.HourStartUtc == h,
                ct);
    }
    public Task SaveChangesAsync(CancellationToken ct)
        => _context.SaveChangesAsync(ct);

    private DateTime NormalizeHour(DateTime dt)
        => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, DateTimeKind.Utc);
}