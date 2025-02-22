using System.Security.Claims;
using System.Text.Encodings.Web;

using MediaService.Grpc.Clients;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MediaService.Utils.Handlers;

public class ExternalAuthenticationHandler(
    IAuthGrpcClient authGrpcClient,
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder)
{
    public static readonly string SchemeName = "External";

    private readonly ILogger<ExternalAuthenticationHandler> _logger = loggerFactory.CreateLogger<ExternalAuthenticationHandler>();

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? authorization = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(authorization))
        {
            return AuthenticateResult.NoResult();
        }

        const string bearerPrefix = "Bearer ";
        if (!authorization.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            const string detail = "Header 'Authorization' does not match format 'Bearer <token>'";
            _logger.LogWarning(detail);
            return AuthenticateResult.Fail(detail);
        }

        var token = authorization.Substring(bearerPrefix.Length).Trim();
        if (string.IsNullOrEmpty(token))
        {
            const string detail = "Token is empty or missing after 'Bearer'";
            _logger.LogWarning(detail);
            return AuthenticateResult.Fail(detail);
        }

        try
        {
            var claims = await authGrpcClient.ValidateToken(token);

            var identity = new ClaimsIdentity(
                claims:
                [
                    new Claim(ClaimTypes.Email, claims.Email),
                    new Claim(ClaimTypes.Role, claims.Role)
                ],
                authenticationType: SchemeName
            );

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            const string detail = "JWT validation failed";
            _logger.LogWarning(ex, detail);
            return AuthenticateResult.Fail(detail);
        }
    }
}
