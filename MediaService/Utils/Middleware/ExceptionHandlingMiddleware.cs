using System.Net;
using System.Text.Json;
using MediaService.Utils.Exceptions;

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
            await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
        }
        catch (InternalServerException ex)
        {
            logger.LogError(ex, "Internal server error");
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error");
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
    {
        var response = new { message = exception.Message };
        var json = JsonSerializer.Serialize(response);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(json);
    }
}
