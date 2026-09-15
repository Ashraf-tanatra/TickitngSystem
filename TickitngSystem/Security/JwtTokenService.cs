using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApplicationServices.DTOs.Account;
using ApplicationServices.Interfaces;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace TickitngSystem.Security;

public sealed class JwtTokenService : IAuthTokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SigningCredentials _signingCredentials;
    private readonly TimeSpan _lifetime;

    public JwtTokenService(IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32)
            throw new InvalidOperationException("Jwt:Key must contain at least 32 bytes.");

        _issuer = configuration["Jwt:Issuer"] ?? "TaskFlow.Api";
        _audience = configuration["Jwt:Audience"] ?? "TaskFlow.Frontend";

        var lifetimeMinutes = configuration.GetValue("Jwt:AccessTokenMinutes", 60);
        _lifetime = TimeSpan.FromMinutes(Math.Clamp(lifetimeMinutes, 5, 120));
        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);
    }

    public AuthToken Create(Account account)
    {
        var expiresAtUtc = DateTime.UtcNow.Add(_lifetime);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, account.Email),
            new Claim(JwtClaimNames.AccountId, account.Id.ToString()),
            new Claim(JwtClaimNames.EmployeeId, account.EmployeeId.ToString()),
            new Claim(
                JwtClaimNames.AccountVersion,
                (account.UpdatedAt ?? account.CreatedAt).Ticks.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: _signingCredentials);

        return new AuthToken(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAtUtc);
    }
}
