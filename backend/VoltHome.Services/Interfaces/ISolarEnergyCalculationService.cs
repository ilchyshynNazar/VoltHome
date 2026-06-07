using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Services.Interfaces;

public interface ISolarEnergyCalculationService
{
    Task CalculateHourAsync(DateTime utcHour, CancellationToken ct);

    /// <summary>Same formula as hourly snapshots, without writing to DB.</summary>
    Task<double> EstimateStationHourKwhAsync(Guid stationId, DateTime utcHour, CancellationToken ct);
}