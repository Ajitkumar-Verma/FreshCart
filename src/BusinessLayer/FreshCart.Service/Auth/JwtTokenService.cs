using FreshCart.Common.Auth;
using FreshCart.Common.Configuration;
using FreshCart.Common.Constants;
using FreshCart.IService.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FreshCart.Service.Auth;

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    public AuthResponse CreateToken(UserAccount user, IReadOnlyList<string> roles)
    {
        var settings = options.Value;
        var signingKey = Encoding.UTF8.GetBytes(settings.SigningKey);
        var expires = DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(TokenClaimNames.Name, user.DisplayName)
        };
        claims.AddRange(roles.Select(role => new Claim(TokenClaimNames.Role, role)));
        var token = new JwtSecurityToken(settings.Issuer, settings.Audience, claims,
            expires: expires,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256));
        return new AuthResponse(user.Id, user.Email, user.DisplayName, roles,
            new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}

