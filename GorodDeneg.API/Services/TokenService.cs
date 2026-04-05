using GorodDeneg.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GorodDeneg.API.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(AppUser user);
}

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly UserManager<AppUser> _users;

    public JwtTokenService(IConfiguration config, UserManager<AppUser> users)
    {
        _config = config; _users = users;
    }

    public async Task<string> GenerateTokenAsync(AppUser user)
    {
        var roles = await _users.GetRolesAsync(user);
        var secret = _config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not set");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier,     user.Id.ToString()),
            new("firstName", user.FirstName),
            new("lastName",  user.LastName),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var expiry = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "1440");
        var token = new JwtSecurityToken(
            issuer:   _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims:   claims,
            expires:  DateTime.UtcNow.AddMinutes(expiry),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
