using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Contracts.Responses.Auth;

public class LoginResponse
{
    public string AccessToken { get; init; }

    public string RefreshToken { get; init; }
    public DateTime RefreshTokenValidTo { get; init; }
}
