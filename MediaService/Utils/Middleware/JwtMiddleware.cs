using MediaService.Grpc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MediaService.Utils.Middleware;

public class JwtMiddleware(IAuthGrpcClient authGrpcClient) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var bearer = context.Request.Headers.Authorization.ToString();
        var token = bearer.Replace("Bearer ", string.Empty);

        if (!string.IsNullOrEmpty(token))
        {
            var claims = await authGrpcClient.ValidateToken(token);
            context.Items["email"] = claims.Email;
            context.Items["role"] = claims.Role;

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

        await next(context);
    }
}