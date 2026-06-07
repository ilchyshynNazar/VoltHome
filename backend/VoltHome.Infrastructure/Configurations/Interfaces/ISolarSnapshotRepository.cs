using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;

namespace VoltHome.Infrastructure.Configurations.Interfaces;

public interface ISolarSnapshotRepository
{
    Task<bool> ExistsAsync(Guid stationId, DateTime hour, CancellationToken ct);

    Task AddAsync(SolarStationEnergySnapshot snapshot, CancellationToken ct);

    Task<List<SolarStationEnergySnapshot>> GetByRangeAsync(Guid stationId, DateTime from, DateTime to, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);

    Task<SolarStationEnergySnapshot?> GetByHourAsync(
        Guid stationId,
        DateTime hour,
        CancellationToken ct);
}