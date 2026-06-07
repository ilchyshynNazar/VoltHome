using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Contracts.Requests.Auth;

public record RefreshTokenRequest(
    string RefreshToken);
