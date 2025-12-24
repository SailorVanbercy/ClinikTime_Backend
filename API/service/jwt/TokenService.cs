using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinikTime.utils;
using Domain.models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ClinikTime.service.jwt;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwt;

    public TokenService(IOptions<JwtSettings> options)
    {
        _jwt = options.Value;
    }
    public string GenerateToken(Utilisateur utilisateur)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, utilisateur.Id.ToString()),
            new Claim(ClaimTypes.Name, utilisateur.Email),
            new Claim(ClaimTypes.Role, utilisateur.Role)
        };
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwt.SecretKey)
        );
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}