using AuthService.Utils.Configuration;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Utils.Jwt;

public class JwtBuilder(IOptions<JwtOptions> options, TimeProvider timeProvider) : IJwtBuilder
{
    private readonly JwtOptions _options = options.Value;
    private readonly TimeProvider _timeProvider = timeProvider;

    public string GetToken(UserClaimsData userClaimsData)
    {
        if (string.IsNullOrWhiteSpace(userClaimsData.Email)) throw new ArgumentException("Email cannot be null or empty", nameof(userClaimsData));
        if (string.IsNullOrWhiteSpace(userClaimsData.Role)) throw new ArgumentException("Role cannot be null or empty", nameof(userClaimsData));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("userEmail", userClaimsData.Email),
            new("userRole", userClaimsData.Role)
        };

        var now = _timeProvider.GetUtcNow();
        var expirationDate = now.AddSeconds(_options.ExpirySeconds);

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expirationDate.UtcDateTime,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public UserClaimsData ValidateToken(string token)
    {
        var principal = GetPrincipal(token);

        if (principal?.Identity == null) throw new SecurityTokenException("Invalid token or principal could not be obtained");

        var identity = (ClaimsIdentity)principal.Identity;

        var userEmailClaim = identity.FindFirst("userEmail");
        var userRoleClaim = identity.FindFirst("userRole");

        if (userEmailClaim == null || userRoleClaim == null) throw new SecurityTokenException("Required claims are missing");

        return new UserClaimsData
        {
            Email = userEmailClaim.Value,
            Role = userRoleClaim.Value
        };
    }

    private ClaimsPrincipal? GetPrincipal(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken == null) return null;

            var key = Encoding.UTF8.GetBytes(_options.Secret);

            var parameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidIssuer = _options.Issuer,
                ValidateAudience = true,
                ValidAudience = _options.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, parameters, out _);
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
