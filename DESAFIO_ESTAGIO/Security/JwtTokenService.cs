using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DESAFIO_ESTAGIO.Contracts;
using DESAFIO_ESTAGIO.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DESAFIO_ESTAGIO.Security;

public static class Roles
{
    public const string Administrator = "Administrator";
    public const string User = "User";
}

public sealed class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 30;
}

public interface ITokenService
{
    TokenResponse Create(ApplicationUser user);
}

public sealed class JwtTokenService(IOptions<JwtOptions> options, TimeProvider clock) : ITokenService
{
    public TokenResponse Create(ApplicationUser user)
    {
        var settings = options.Value;
        var now = clock.GetUtcNow();
        var expires = now.AddMinutes(settings.ExpirationMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", user.IsAdministrator ? Roles.Administrator : Roles.User),
            new Claim("security_stamp", user.SecurityStamp!)
        };
        var token = new JwtSecurityToken(settings.Issuer, settings.Audience, claims,
            now.UtcDateTime, expires.UtcDateTime,
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)), SecurityAlgorithms.HmacSha256));
        return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), "Bearer", expires);
    }
}
