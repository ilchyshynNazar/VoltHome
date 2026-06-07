using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoltHome.Contracts.Responses.SolarStation;
using VoltHome.Services.Interfaces;

namespace VoltHome.API.Controllers;

[ApiController]
[Route("api/solar")]
[Authorize]
public class SolarDashboardController : ControllerBase
{
    private readonly ISolarDashboardService _dashboard;
    private readonly ISolarEnergyCalculationService _calc;
    private readonly ISolarPaybackService _payback;

    public SolarDashboardController(
        ISolarDashboardService dashboard,
        ISolarEnergyCalculationService calc,
        ISolarPaybackService payback)
    {
        _dashboard = dashboard;
        _calc = calc;
        _payback = payback;
    }

    [HttpGet("{stationId:guid}/dashboard")]
    public async Task<ActionResult<SolarDashboardDto>> GetDashboard(
        Guid stationId,
        CancellationToken ct)
    {
        var result = await _dashboard.GetDashboardAsync(stationId, ct);
        return Ok(result);
    }

    /// <summary>Green tariff / payback estimates and monthly irradiation profile.</summary>
    [HttpGet("{stationId:guid}/insights")]
    public async Task<ActionResult<SolarPaybackInsightDto>> GetInsights(
        Guid stationId,
        CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _payback.GetInsightsAsync(stationId, userId, ct);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("debug/calculate")]
    public async Task<IActionResult> Calculate(
        [FromBody] DateTime hourUtc,
        CancellationToken ct)
    {
        await _calc.CalculateHourAsync(hourUtc, ct);
        return Ok("Calculated");
    }
}
