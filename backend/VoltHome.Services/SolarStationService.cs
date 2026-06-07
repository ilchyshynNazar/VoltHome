using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Contracts.Requests.SolarStation;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure;
using VoltHome.Services.Interfaces;

namespace VoltHome.Services;

public class SolarStationService : ISolarStationService
{
    private readonly AppDbContext _context;

    public SolarStationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(Guid userId, CreateSolarStationRequest request, CancellationToken ct)
    {
        var regionExists = await _context.SolarRegions
            .AnyAsync(x => x.Id == request.SolarRegionId, ct);

        if (!regionExists)
            throw new Exception("Invalid region");

        var station = new SolarStation
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            OwnerId = userId,
            SolarRegionId = request.SolarRegionId,
            CreatedAt = DateTime.UtcNow,

            Inverter = new Inverter
            {
                Id = Guid.NewGuid(),
                Power = request.Inverter.Power,
                Efficiency = request.Inverter.Efficiency
            },

            PanelGroups = request.PanelGroups.Select(pg => new PanelGroup
            {
                Id = Guid.NewGuid(),
                PanelCount = pg.PanelCount,
                PowerPerPanel = pg.PowerPerPanel,
                TiltAngle = pg.TiltAngle,
                Azimuth = pg.Azimuth
            }).ToList()
        };

        await _context.SolarStations.AddAsync(station, ct);
        await _context.SaveChangesAsync(ct);

        return station.Id;
    }

    public async Task<SolarStationDto> GetByIdAsync(Guid id, Guid userId, CancellationToken ct)
    {
        var station = await _context.SolarStations
            .Include(x => x.PanelGroups)
            .Include(x => x.Inverter)
            .Include(x => x.SolarRegion)
            .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId, ct);

        if (station == null)
            throw new Exception("Station not found");

        return MapToDto(station);
    }

    public async Task<List<SolarStationDto>> GetUserStationsAsync(Guid userId, CancellationToken ct)
    {
        var stations = await _context.SolarStations
            .Include(x => x.PanelGroups)
            .Include(x => x.Inverter)
            .Include(x => x.SolarRegion)
            .Where(x => x.OwnerId == userId)
            .ToListAsync(ct);

        return stations.Select(MapToDto).ToList();
    }

    public async Task UpdateAsync(Guid id, Guid userId, UpdateSolarStationRequest request, CancellationToken ct)
    {
        var station = await _context.SolarStations
            .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId, ct);

        if (station == null)
            throw new Exception("Station not found");

        station.Name = request.Name;
        station.SolarRegionId = request.SolarRegionId;

        _context.SolarStations.Update(station);

        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct)
    {
        var station = await _context.SolarStations
            .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId, ct);

        if (station == null)
            throw new Exception("Station not found");

        _context.SolarStations.Remove(station);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AddPanelGroupAsync(Guid stationId, Guid userId, CreatePanelGroupDto dto, CancellationToken ct)
    {
        var stationExists = await _context.SolarStations
            .AnyAsync(x => x.Id == stationId && x.OwnerId == userId, ct);

        if (!stationExists)
            throw new Exception("Station not found");

        var group = new PanelGroup
        {
            Id = Guid.NewGuid(),
            SolarStationId = stationId,
            PanelCount = dto.PanelCount,
            PowerPerPanel = dto.PowerPerPanel,
            TiltAngle = dto.TiltAngle,
            Azimuth = dto.Azimuth
        };

        await _context.PanelGroups.AddAsync(group, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemovePanelGroupAsync(Guid stationId, Guid groupId, Guid userId, CancellationToken ct)
    {
        var group = await _context.PanelGroups
            .Include(x => x.SolarStation)
            .FirstOrDefaultAsync(x =>
                x.Id == groupId &&
                x.SolarStationId == stationId &&
                x.SolarStation.OwnerId == userId, ct);

        if (group == null)
            throw new Exception("Group not found");

        _context.PanelGroups.Remove(group);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateInverterAsync(Guid stationId, Guid userId, CreateInverterDto dto, CancellationToken ct)
    {
        var station = await _context.SolarStations
            .Include(x => x.Inverter)
            .FirstOrDefaultAsync(x => x.Id == stationId && x.OwnerId == userId, ct);

        if (station?.Inverter == null)
            throw new Exception("Station not found");

        station.Inverter.Power = dto.Power;
        station.Inverter.Efficiency = dto.Efficiency;

        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdatePanelGroupAsync(
        Guid stationId,
        Guid groupId,
        Guid userId,
        CreatePanelGroupDto dto,
        CancellationToken ct)
    {
        var group = await _context.PanelGroups
            .Include(x => x.SolarStation)
            .FirstOrDefaultAsync(x =>
                x.Id == groupId &&
                x.SolarStationId == stationId &&
                x.SolarStation.OwnerId == userId, ct);

        if (group == null)
            throw new Exception("Group not found");

        group.PanelCount = dto.PanelCount;
        group.PowerPerPanel = dto.PowerPerPanel;
        group.TiltAngle = dto.TiltAngle;
        group.Azimuth = dto.Azimuth;

        await _context.SaveChangesAsync(ct);
    }

    private static SolarStationDto MapToDto(SolarStation station)
    {
        return new SolarStationDto
        {
            Id = station.Id,
            Name = station.Name,
            SolarRegionId = station.SolarRegionId,
            SolarRegionName = station.SolarRegion?.Name ?? string.Empty,
            Inverter = station.Inverter == null
                ? null
                : new InverterDto
                {
                    Power = station.Inverter.Power,
                    Efficiency = station.Inverter.Efficiency
                },
            PanelGroups = station.PanelGroups?
                .Select(pg => new PanelGroupDto
                {
                    Id = pg.Id,
                    PanelCount = pg.PanelCount,
                    PowerPerPanel = pg.PowerPerPanel,
                    TiltAngle = pg.TiltAngle,
                    Azimuth = pg.Azimuth
                })
                .ToList()
        };
    }
}