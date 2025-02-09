using Microsoft.AspNetCore.Authentication;

using System.Net;

using MediaService.Utils.Handlers;

namespace MediaService.Utils.Middleware;

public class AuthorizationHandlingMiddleware(
    ILogger<ExceptionHandlingMiddleware> logger,
    RequestDelegate next
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        string? errorMessage = null;
        var authenticateResult = await context.AuthenticateAsync();
        if (authenticateResult.Failure != null)
        {
            errorMessage = authenticateResult.Failure.Message;
        }

        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            var detail = errorMessage ?? "Unauthorized";
            logger.LogWarning($"Unauthorized: {detail}");
            await ErrorResponseHandler.HandleErrorAsync(context, HttpStatusCode.Unauthorized, detail);
        }
        else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
        {
            var detail = errorMessage ?? "Forbidden";
            logger.LogWarning($"Forbidden: {detail}");
            await ErrorResponseHandler.HandleErrorAsync(context, HttpStatusCode.Forbidden, detail);
        }
    }
}
