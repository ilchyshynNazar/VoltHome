using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoltHome.Contracts.Requests.SolarStation;
using VoltHome.Services.Interfaces;

namespace VoltHome.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolarStationsController : ControllerBase
{
    private readonly ISolarStationService _service;

    public SolarStationsController(ISolarStationService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSolarStationRequest request,
        CancellationToken ct)
    {
        var userId = GetUserId();

        var id = await _service.CreateAsync(userId, request, ct);

        return Ok(new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();

        var station = await _service.GetByIdAsync(id, userId, ct);

        return Ok(station);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserStations(CancellationToken ct)
    {
        var userId = GetUserId();

        var stations = await _service.GetUserStationsAsync(userId, ct);

        return Ok(stations);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSolarStationRequest request,
        CancellationToken ct)
    {
        var userId = GetUserId();

        await _service.UpdateAsync(id, userId, request, ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();

        await _service.DeleteAsync(id, userId, ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/panel-groups")]
    public async Task<IActionResult> AddPanelGroup(
        Guid id,
        [FromBody] CreatePanelGroupDto request,
        CancellationToken ct)
    {
        var userId = GetUserId();

        await _service.AddPanelGroupAsync(id, userId, request, ct);

        return Ok();
    }

    [HttpDelete("{stationId:guid}/panel-groups/{groupId:guid}")]
    public async Task<IActionResult> RemovePanelGroup(
        Guid stationId,
        Guid groupId,
        CancellationToken ct)
    {
        var userId = GetUserId();

        await _service.RemovePanelGroupAsync(stationId, groupId, userId, ct);

        return NoContent();
    }

    [HttpPut("{stationId:guid}/inverter")]
    public async Task<IActionResult> UpdateInverter(
        Guid stationId,
        [FromBody] CreateInverterDto request,
        CancellationToken ct)
    {
        var userId = GetUserId();

        await _service.UpdateInverterAsync(stationId, userId, request, ct);

        return NoContent();
    }

    [HttpPut("{stationId:guid}/panel-groups/{groupId:guid}")]
    public async Task<IActionResult> UpdatePanelGroup(
        Guid stationId,
        Guid groupId,
        [FromBody] CreatePanelGroupDto request,
        CancellationToken ct)
    {
        var userId = GetUserId();

        await _service.UpdatePanelGroupAsync(stationId, groupId, userId, request, ct);

        return NoContent();
    }
}
