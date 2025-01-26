using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using MediaService.Utils.Exceptions;
using MediaService.Utils.Handles;

namespace MediaService.Utils.Middleware;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ResourceNotFoundException ex)
        {
            logger.LogWarning(ex, "Resource not found");
            await ErrorResponseHandler.HandleErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (InternalServerException ex)
        {
            logger.LogError(ex, "Internal server error");
            await ErrorResponseHandler.HandleErrorAsync(context, HttpStatusCode.InternalServerError, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error");
            await ErrorResponseHandler.HandleErrorAsync(context, HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
