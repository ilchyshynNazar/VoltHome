using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Contracts.Responses.Auth;

public record RefreshTokenResponse
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
    public DateTime RefreshTokenValidTo { get; init; }
}
