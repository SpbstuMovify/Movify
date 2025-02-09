using System.Text.Json;

using JetBrains.Annotations;

using MediaService.Utils.Middleware;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace MediaService.Tests.Utils.Middleware;

[TestSubject(typeof(AuthorizationHandlingMiddleware))]
public class AuthorizationHandlingMiddlewareTest
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock = new();

    private HttpContext CreateContext(AuthenticateResult authResult)
    {
        var services = new ServiceCollection();
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(authResult);

        services.AddSingleton(authServiceMock.Object);

        var provider = services.BuildServiceProvider();

        var context = new DefaultHttpContext
        {
            RequestServices = provider,
            Response =
            {
                Body = new MemoryStream()
            }
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
    public async Task InvokeAsync_WhenStatusNot401Or403_DoesNothing()
    {
        // Arrange
        var middleware = new AuthorizationHandlingMiddleware(_loggerMock.Object, _ => Task.CompletedTask);
        var authResult = AuthenticateResult.NoResult();
        var context = CreateContext(authResult);

        context.Response.StatusCode = 200;

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WhenStatus401_UsesAuthenticateFailureMessage_IfPresent()
    {
        // Arrange
        var failureException = new InvalidOperationException("Some auth error");
        var failureAuthResult = AuthenticateResult.Fail(failureException);

        var context = CreateContext(failureAuthResult);

        var middleware = new AuthorizationHandlingMiddleware(_loggerMock.Object, Next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(401, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        var root = await ReadJsonResponse(context);
        Assert.Equal("UNAUTHORIZED", root.GetProperty("statusCode").GetString());

        var body = root.GetProperty("body");
        Assert.Equal(401, body.GetProperty("status").GetInt32());
        Assert.Equal("Unauthorized", body.GetProperty("title").GetString());
        var detail = body.GetProperty("detail").GetString();
        Assert.Equal("Some auth error", detail);
        
        return;

        Task Next(HttpContext ctx)
        {
            ctx.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenStatus401_NoFailureMessage_UsesUnauthorizedAsDetail()
    {
        // Arrange
        var failureAuthResult = AuthenticateResult.Fail(((Exception?)null)!);

        var context = CreateContext(failureAuthResult);

        var middleware = new AuthorizationHandlingMiddleware(_loggerMock.Object, Next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(401, context.Response.StatusCode);
        var root = await ReadJsonResponse(context);

        Assert.Equal("UNAUTHORIZED", root.GetProperty("statusCode").GetString());
        var body = root.GetProperty("body");
        Assert.Equal("Unauthorized", body.GetProperty("detail").GetString());
        
        return;

        Task Next(HttpContext ctx)
        {
            ctx.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenStatus403_UsesAuthenticateFailureMessage_IfPresent()
    {
        // Arrange
        var failureException = new UnauthorizedAccessException("You have no power here!");
        var failureAuthResult = AuthenticateResult.Fail(failureException);

        var context = CreateContext(failureAuthResult);

        var middleware = new AuthorizationHandlingMiddleware(_loggerMock.Object, Next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(403, context.Response.StatusCode);
        var root = await ReadJsonResponse(context);

        Assert.Equal("FORBIDDEN", root.GetProperty("statusCode").GetString());
        var body = root.GetProperty("body");
        Assert.Equal(403, body.GetProperty("status").GetInt32());
        Assert.Equal("Forbidden", body.GetProperty("title").GetString());
        Assert.Equal("You have no power here!", body.GetProperty("detail").GetString());
        
        return;

        Task Next(HttpContext ctx)
        {
            ctx.Response.StatusCode = 403;
            return Task.CompletedTask;
        }
    }
}
