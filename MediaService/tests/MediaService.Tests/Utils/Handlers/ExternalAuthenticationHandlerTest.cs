using System.Security.Claims;
using System.Text.Encodings.Web;

using Grpc.Core;

using JetBrains.Annotations;

using MediaService.Dtos.Claims;
using MediaService.Grpc.Clients;
using MediaService.Utils.Handlers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace MediaService.Tests.Utils.Handlers;

[TestSubject(typeof(ExternalAuthenticationHandler))]
public class ExternalAuthenticationHandlerTest
{
    private readonly Mock<IAuthGrpcClient> _authGrpcClientMock;
    private readonly Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _optionsMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;

    public ExternalAuthenticationHandlerTest()
    {
        _authGrpcClientMock = new Mock<IAuthGrpcClient>();

        _optionsMock = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
        _optionsMock
            .Setup(o => o.Get(ExternalAuthenticationHandler.SchemeName))
            .Returns(new AuthenticationSchemeOptions());

        var loggerMock = new Mock<ILogger<ExternalAuthenticationHandler>>();
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _loggerFactoryMock
            .Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(loggerMock.Object);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WhenNoAuthorizationHeader_ReturnsFailAndLogsWarning()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var handler = await CreateHandlerAsync(context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(result.Failure?.Message);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WhenInvalidPrefix_ReturnsFail()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Token xyz";
        var handler = await CreateHandlerAsync(context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Header 'Authorization' does not match format 'Bearer <token>'", result.Failure?.Message);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WhenBearerPrefixButEmptyToken_ReturnsFail()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Bearer ";
        var handler = await CreateHandlerAsync(context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Token is empty or missing after 'Bearer'", result.Failure?.Message);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WhenAuthGrpcClientThrows_ReturnsFail()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Bearer validToken";
        var handler = await CreateHandlerAsync(context);
        
        _authGrpcClientMock
            .Setup(c => c.ValidateToken("validToken"))
            .ThrowsAsync(new RpcException(new Status(StatusCode.Unauthenticated, "Invalid token")));

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("JWT validation failed", result.Failure?.Message);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WhenAuthGrpcClientSucceeds_ReturnsSuccess()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Bearer realJwt";
        var handler = await CreateHandlerAsync(context);

        var claimsDto = new ClaimsDto("user@example.com", "USER");
        _authGrpcClientMock
            .Setup(c => c.ValidateToken("realJwt"))
            .ReturnsAsync(claimsDto);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Principal);
        Assert.Equal(ExternalAuthenticationHandler.SchemeName, result.Ticket?.AuthenticationScheme);

        var identity = result.Principal?.Identity as ClaimsIdentity;
        Assert.NotNull(identity);
        Assert.Equal(2, identity.Claims.Count());

        var emailClaim = identity.FindFirst(ClaimTypes.Email);
        Assert.NotNull(emailClaim);
        Assert.Equal("user@example.com", emailClaim.Value);

        var roleClaim = identity.FindFirst(ClaimTypes.Role);
        Assert.NotNull(roleClaim);
        Assert.Equal("USER", roleClaim.Value);
    }

    private async Task<ExternalAuthenticationHandler> CreateHandlerAsync(HttpContext context)
    {
        var handler = new ExternalAuthenticationHandler(
            _authGrpcClientMock.Object,
            _optionsMock.Object,
            _loggerFactoryMock.Object,
            UrlEncoder.Default
        );

        var scheme = new AuthenticationScheme(
            ExternalAuthenticationHandler.SchemeName,
            ExternalAuthenticationHandler.SchemeName,
            typeof(ExternalAuthenticationHandler)
        );

        await handler.InitializeAsync(scheme, context);
        return handler;
    }
}
