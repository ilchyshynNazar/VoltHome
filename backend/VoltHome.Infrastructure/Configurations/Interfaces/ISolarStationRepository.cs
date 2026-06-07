using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure.Repositories;

namespace VoltHome.Infrastructure.Configurations.Interfaces;

public interface ISolarStationRepository
{
    Task<SolarStation?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct);

    Task<List<SolarStation>> GetUserStationsAsync(Guid userId, CancellationToken ct);

    Task<List<SolarStation>> GetStationsForCalculationAsync(CancellationToken ct);

    Task AddAsync(SolarStation station, CancellationToken ct);

    void Update(SolarStation station);

    void Remove(SolarStation station);
}