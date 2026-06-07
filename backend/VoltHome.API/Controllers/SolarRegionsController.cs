using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoltHome.Infrastructure;

namespace VoltHome.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolarRegionsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var regions = await context.SolarRegions
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(ct);

        return Ok(regions);
    }
}
