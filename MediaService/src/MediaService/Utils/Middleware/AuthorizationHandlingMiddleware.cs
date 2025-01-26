using MediaService.Utils.Handles;
using Microsoft.AspNetCore.Authentication;
using System.Net;

namespace MediaService.Utils.Middleware;

public class AuthorizationHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next)
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
            string detail = errorMessage ?? "Unauthorized";
            logger.LogWarning($"Unauthorized: {detail}");
            await ErrorResponseHandler.HandleErrorAsync(context, HttpStatusCode.Unauthorized, detail);
        }
        else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
        {
            string detail = errorMessage ?? "Forbidden";
            logger.LogWarning($"Forbidden: {detail}");
            await ErrorResponseHandler.HandleErrorAsync(context, HttpStatusCode.Forbidden, detail);
        }
    }
}
