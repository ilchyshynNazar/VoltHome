using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Contracts.Responses.SolarStation;

namespace VoltHome.Services.Interfaces;

public interface ISolarDashboardService
{
    Task<SolarDashboardDto> GetDashboardAsync(Guid stationId, CancellationToken ct);
}
