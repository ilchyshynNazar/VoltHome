using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Services.Interfaces;

public interface IRnnNetworkService
{
    public Task<float> PredictNextHourAsync(
            Guid stationId,
            CancellationToken ct);
}
