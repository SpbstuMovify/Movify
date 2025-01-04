using MediaService.Grpc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MediaService.Utils.Middleware;

public class JwtMiddleware(ILogger<JwtMiddleware> logger, IAuthGrpcClient authGrpcClient) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var bearer = context.Request.Headers.Authorization.ToString();
        var token = bearer.Replace("Bearer ", string.Empty);

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var claims = await authGrpcClient.ValidateToken(token);

                if (claims != null)
                {
                    var identity = new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.Email, claims.Email),
                    new Claim(ClaimTypes.Role, claims.Role)
                    ], "jwt");

                    context.User = new ClaimsPrincipal(identity);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "JWT validation failed");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }

        await next(context);
    }
}