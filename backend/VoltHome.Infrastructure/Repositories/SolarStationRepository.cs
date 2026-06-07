using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure.Configurations.Interfaces;

namespace VoltHome.Infrastructure.Repositories;

public class SolarStationRepository : ISolarStationRepository
{
    private readonly AppDbContext _context;

    public SolarStationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SolarStation?> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken ct)
    {
        return await _context.SolarStations
            .Include(x => x.PanelGroups)
            .Include(x => x.Inverter)
            .Include(x => x.SolarRegion)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.OwnerId == userId,
                ct);
    }

    public async Task<List<SolarStation>> GetUserStationsAsync(
        Guid userId,
        CancellationToken ct)
    {
        return await _context.SolarStations
            .Include(x => x.PanelGroups)
            .Where(x => x.OwnerId == userId)
            .ToListAsync(ct);
    }

    public async Task<List<SolarStation>> GetStationsForCalculationAsync(CancellationToken ct)
    {
        return await _context.SolarStations
            .Include(x => x.PanelGroups)
            .Include(x => x.Inverter)
            .Include(x => x.SolarRegion)
            .ToListAsync(ct);
    }

    public async Task AddAsync(
        SolarStation station,
        CancellationToken ct)
    {
        await _context.SolarStations.AddAsync(station, ct);
    }

    public void Update(SolarStation station)
    {
        _context.SolarStations.Update(station);
    }

    public void Remove(SolarStation station)
    {
        _context.SolarStations.Remove(station);
    }
}
