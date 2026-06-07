using System;
using System.Threading;
using System.Threading.Tasks;
using VoltHome.Contracts.Responses.SolarStation;

namespace VoltHome.Services.Interfaces;

public interface ISolarPaybackService
{
    Task<SolarPaybackInsightDto> GetInsightsAsync(Guid stationId, Guid ownerUserId, CancellationToken ct);
}
