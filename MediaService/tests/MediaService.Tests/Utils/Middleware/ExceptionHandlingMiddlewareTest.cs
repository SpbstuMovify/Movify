using System.Net;
using System.Text.Json;

using FluentValidation;

using JetBrains.Annotations;

using MediaService.Utils.Exceptions;
using MediaService.Utils.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Moq;

namespace MediaService.Tests.Utils.Middleware;

[TestSubject(typeof(ExceptionHandlingMiddleware))]
public class ExceptionHandlingMiddlewareTest
{

    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock = new();
    
    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };
        return context;
    }
    
    private static async Task<JsonElement> ReadJsonResponse(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var json = await reader.ReadToEndAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    [Fact]
    public async Task Invoke_NoException_DoesNothing()
    {
        // Arrange
        var middleware = new ExceptionHandlingMiddleware(_loggerMock.Object, Next);
        var context = CreateHttpContext();

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal(200, context.Response.StatusCode);
        
        return;
        
        Task Next(HttpContext ctx)
        {
            ctx.Response.StatusCode = 200;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Invoke_WhenNotFoundException_LogsWarning_AndReturns404()
    {
        // Arrange
        const string notFoundMessage = "Resource XYZ not found";

        var middleware = new ExceptionHandlingMiddleware(_loggerMock.Object, Next);
        var context = CreateHttpContext();

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        var root = await ReadJsonResponse(context);
        Assert.Equal("NOT_FOUND", root.GetProperty("statusCode").GetString());
        var body = root.GetProperty("body");
        Assert.Equal(404, body.GetProperty("status").GetInt32());
        Assert.Equal("Resource XYZ not found", body.GetProperty("detail").GetString());
        Assert.Equal("Not Found", body.GetProperty("title").GetString());
        
        return;

        Task Next(HttpContext _) => throw new NotFoundException(notFoundMessage);
    }

    [Fact]
    public async Task Invoke_WhenValidationException_LogsWarning_AndReturns400()
    {
        // Arrange
        const string validationMessage = "Invalid input data";

        var middleware = new ExceptionHandlingMiddleware(_loggerMock.Object, Next);
        var context = CreateHttpContext();

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        var root = await ReadJsonResponse(context);
        Assert.Equal("BAD_REQUEST", root.GetProperty("statusCode").GetString());
        var body = root.GetProperty("body");
        Assert.Equal(400, body.GetProperty("status").GetInt32());
        Assert.Equal(validationMessage, body.GetProperty("detail").GetString());
        Assert.Equal("Bad Request", body.GetProperty("title").GetString());
        
        return;

        Task Next(HttpContext _) => throw new ValidationException(validationMessage);
    }

    [Fact]
    public async Task Invoke_WhenInternalServerErrorException_LogsError_AndReturns500()
    {
        // Arrange
        const string message = "Some internal error occurred";

        var middleware = new ExceptionHandlingMiddleware(_loggerMock.Object, Next);
        var context = CreateHttpContext();

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        var root = await ReadJsonResponse(context);
        Assert.Equal("INTERNAL_SERVER_ERROR", root.GetProperty("statusCode").GetString());
        var body = root.GetProperty("body");
        Assert.Equal(500, body.GetProperty("status").GetInt32());
        Assert.Equal(message, body.GetProperty("detail").GetString());
        Assert.Equal("Internal Server Error", body.GetProperty("title").GetString());
        
        return;

        Task Next(HttpContext _) => throw new InternalServerErrorException(message);
    }

    [Fact]
    public async Task Invoke_WhenUnknownException_LogsError_AndReturns500()
    {
        // Arrange
        const string exMessage = "Something unexpected";

        var middleware = new ExceptionHandlingMiddleware(_loggerMock.Object, Next);
        var context = CreateHttpContext();

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        var root = await ReadJsonResponse(context);
        Assert.Equal("INTERNAL_SERVER_ERROR", root.GetProperty("statusCode").GetString());
        var body = root.GetProperty("body");
        Assert.Equal(500, body.GetProperty("status").GetInt32());
        Assert.Equal("Something unexpected", body.GetProperty("detail").GetString());
        Assert.Equal("Internal Server Error", body.GetProperty("title").GetString());
        
        return;

        Task Next(HttpContext _) => throw new Exception(exMessage);
    }
}
