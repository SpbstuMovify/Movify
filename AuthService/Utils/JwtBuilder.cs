using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthMicroservice.Utils;

public class JwtBuilder(IOptions<JwtOptions> options) : IJwtBuilder
{
    private readonly JwtOptions _options = options.Value;

    public string GetToken(string email, bool isAdmin)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim("userEmail", email),
            new Claim("userIsAdmin", isAdmin.ToString())
        };
        var expirationDate = DateTime.Now.AddMinutes(_options.ExpiryMinutes);
        var jwt = new JwtSecurityToken(claims: claims, signingCredentials: signingCredentials, expires: expirationDate);
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }

    public (string, bool) ValidateToken(string token)
    {
        var principal = GetPrincipal(token);
        if (principal == null)
        {
            return (string.Empty, false);
        }

        ClaimsIdentity identity;

        if (principal.Identity == null)
        {
            return (string.Empty, false);
        }

        identity = (ClaimsIdentity)principal.Identity;

        var userEmailClaim = identity.FindFirst("userEmail");
        var userIsAdminClaim = identity.FindFirst("userIsAdmin");
        if (userEmailClaim == null || userIsAdminClaim == null)
        {
            return (string.Empty, false);
        }
        
        var userEmail = userEmailClaim.Value;
        var userIsAdmin = bool.Parse(userIsAdminClaim.Value);
        return (userEmail, userIsAdmin);
    }

    private ClaimsPrincipal? GetPrincipal(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            if (jwtToken == null)
            {
                return null;
            }
            var key = Encoding.UTF8.GetBytes(_options.Secret);
            var parameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            IdentityModelEventSource.ShowPII = true;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out _);
            return principal;
        }
        catch (Exception)
        {
            return null;
        }
    }
}