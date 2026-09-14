using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WorkPortal.Api.Entities;

namespace WorkPortal.Api.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateAccessToken(AppUser user);
    string CreateRefreshToken();
}

public class TokenService : ITokenService
{
    private readonly JwtOptions options;

    public TokenService(IOptions<JwtOptions> options) => this.options = options.Value;

    public (string Token, DateTime ExpiresAt) CreateAccessToken(AppUser user)
    {
        Claim[] claims =
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(options.Key));
        DateTime expires = DateTime.UtcNow.AddMinutes(options.AccessTokenMinutes);

        JwtSecurityToken token = new(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public string CreateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
}
