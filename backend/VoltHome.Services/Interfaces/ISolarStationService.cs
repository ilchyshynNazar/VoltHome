using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Contracts.Requests.SolarStation;

namespace VoltHome.Services.Interfaces;

public interface ISolarStationService
{
    Task<Guid> CreateAsync(Guid userId, CreateSolarStationRequest request, CancellationToken ct);

    Task<SolarStationDto> GetByIdAsync(Guid id, Guid userId, CancellationToken ct);

    Task<List<SolarStationDto>> GetUserStationsAsync(Guid userId, CancellationToken ct);

    Task UpdateAsync(Guid id, Guid userId, UpdateSolarStationRequest request, CancellationToken ct);

    Task DeleteAsync(Guid id, Guid userId, CancellationToken ct);

    Task AddPanelGroupAsync(Guid stationId, Guid userId, CreatePanelGroupDto dto, CancellationToken ct);

    Task RemovePanelGroupAsync(Guid stationId, Guid groupId, Guid userId, CancellationToken ct);

    Task UpdateInverterAsync(Guid stationId, Guid userId, CreateInverterDto dto, CancellationToken ct);

    Task UpdatePanelGroupAsync(
        Guid stationId,
        Guid groupId,
        Guid userId,
        CreatePanelGroupDto dto,
        CancellationToken ct);
}