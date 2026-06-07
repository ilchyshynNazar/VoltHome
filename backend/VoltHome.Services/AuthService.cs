using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VoltHome.Contracts.Requests.Auth;
using VoltHome.Contracts.Responses.Auth;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure;
using VoltHome.Services.Interfaces;

namespace VoltHome.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    AppDbContext context,
    IConfiguration configuration)
    : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly AppDbContext _context = context;
    private readonly IConfiguration _configuration = configuration;

    public async Task<LoginResponse> LoginAsync(AuthLoginRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid login or password");

        var claims = BuildClaims(user);

        var accessToken = GenerateJwtToken(claims);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenValidTo = DateTime.UtcNow.AddDays(
            int.Parse(_configuration["Jwt:RefreshExpiresInDays"])
        );

        var tokenWithExpiration = $"{refreshToken}|{refreshTokenValidTo:o}";

        await _userManager.SetAuthenticationTokenAsync(
            user,
            "VoltHome",
            "RefreshToken",
            tokenWithExpiration
        );

        return new LoginResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
            RefreshToken = refreshToken,
            RefreshTokenValidTo = refreshTokenValidTo
        };
    }

    public async Task<ApplicationUser> RegisterAsync(RegisterUserRequest request, CancellationToken ct)
    {
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            RegisteredAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        return user;
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken ct)
    {
        var userToken = await _context.Set<IdentityUserToken<Guid>>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t =>
                t.LoginProvider == "VoltHome" &&
                t.Name == "RefreshToken",
                ct);

        var (userId, _) = ValidateAndParseRefreshToken(userToken, request.RefreshToken);

        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UnauthorizedAccessException("User not found");

        var claims = BuildClaims(user);

        var newAccessToken = GenerateJwtToken(claims);

        var newRefreshToken = GenerateRefreshToken();
        var newRefreshTokenValidTo = DateTime.UtcNow.AddDays(
            int.Parse(_configuration["Jwt:RefreshExpiresInDays"])
        );

        await _userManager.SetAuthenticationTokenAsync(
            user,
            "VoltHome",
            "RefreshToken",
            $"{newRefreshToken}|{newRefreshTokenValidTo:o}"
        );

        return new RefreshTokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken,
            RefreshTokenValidTo = newRefreshTokenValidTo
        };
    }

    private List<Claim> BuildClaims(ApplicationUser user)
    {
        return new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
    }

    private JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
        );

        return new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpiresIn"])
            ),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private static (Guid UserId, DateTime ValidTo) ValidateAndParseRefreshToken(
        IdentityUserToken<Guid>? userToken,
        string requestToken)
    {
        if (userToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var parts = userToken.Value.Split('|', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
            throw new UnauthorizedAccessException("Invalid refresh token format");

        var savedToken = parts[0];
        var expiresAt = parts[1];

        if (savedToken != requestToken)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (!DateTime.TryParse(expiresAt, out var validTo))
            throw new UnauthorizedAccessException("Invalid expiration");

        if (DateTime.UtcNow > validTo)
            throw new UnauthorizedAccessException("Refresh token expired");

        return (userToken.UserId, validTo);
    }
}