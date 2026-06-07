using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Contracts.Requests.Auth;
using VoltHome.Contracts.Responses.Auth;
using VoltHome.Domain.dbo;

namespace VoltHome.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(AuthLoginRequest request, CancellationToken ct);

    Task<ApplicationUser> RegisterAsync(RegisterUserRequest request, CancellationToken ct);

    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct);
}
