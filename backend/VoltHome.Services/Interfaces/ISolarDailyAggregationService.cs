using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;

namespace VoltHome.Services.Interfaces;

public interface ISolarDailyAggregationService
{
    Task CalculateDayAsync(DateTime day, CancellationToken ct); 
}
