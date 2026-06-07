using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using VoltHome.Contracts.Requests.Auth;
using VoltHome.Contracts.Responses.Auth;
using VoltHome.Services.Interfaces;

namespace VoltHome.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService
) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
            [FromBody] AuthLoginRequest request,
            CancellationToken ct)
    {
        try
        {
            var result = await _authService.LoginAsync(request, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid login or password");
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken ct)
    {
        try
        {
            return Ok(await _authService.RefreshTokenAsync(request, ct));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("register/client")]
    public async Task<IActionResult> RegisterClient(
        [FromBody] RegisterUserRequest request,
        CancellationToken ct)
    {
        try
        {
            var user = await _authService.RegisterAsync(request, ct);
            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
