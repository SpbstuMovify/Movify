using AuthService.Grpc.Interceptors;
using AuthService.Utils.Exceptions;

using Grpc.Core;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Moq;

namespace AuthService.Tests.Grpc.Interceptors;

[TestSubject(typeof(ErrorHandlerInterceptor))]
public class ErrorHandlerInterceptorTest
{
    private readonly ErrorHandlerInterceptor _interceptor;

    private readonly Mock<ILogger<ErrorHandlerInterceptor>> _loggerMock = new();

    public ErrorHandlerInterceptorTest() { _interceptor = new ErrorHandlerInterceptor(_loggerMock.Object); }

    [Fact]
    public async Task UnaryServerHandler_WithNoException_ReturnsResponse()
    {
        // Arrange
        const string request = "request";
        const string expectedResponse = "response";

        var context = new TestServerCallContext(method: "/TestService/UnaryCall");

        // Act
        var result = await _interceptor.UnaryServerHandler(request, context, Continuation);

        // Assert
        Assert.Equal(expectedResponse, result);
        
        return;

        async Task<string> Continuation(
            string req,
            ServerCallContext ctx
        )
        {
            await Task.Delay(10);
            return expectedResponse;
        }
    }

    [Fact]
    public async Task UnaryServerHandler_WithAuthServiceException_ThrowsRpcExceptionUnauthenticated()
    {
        // Arrange
        const string request = "request";
        const string exceptionMessage = "Auth error";

        var context = new TestServerCallContext(method: "/TestService/UnaryCall");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.Unauthenticated, exception.StatusCode);
        Assert.Contains(exceptionMessage, exception.Status.Detail);
        
        return;

        Task<string> Continuation(
            string req,
            ServerCallContext ctx
        )
        {
            throw new AuthServiceException(exceptionMessage);
        }
    }

    [Fact]
    public async Task UnaryServerHandler_WithRpcExceptionInvalidArgument_RethrowsAsSameCode()
    {
        // Arrange
        const string request = "request";
        const string exceptionMessage = "Invalid argument details";

        var context = new TestServerCallContext();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains(exceptionMessage, exception.Status.Detail);
        
        return;

        Task<string> Continuation(
            string req,
            ServerCallContext ctx
        )
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exceptionMessage));
        }
    }

    [Fact]
    public async Task UnaryServerHandler_WithRpcExceptionOtherCode_ThrowsRpcExceptionWithInternalCode()
    {
        // Arrange
        const string request = "request";
        const StatusCode originalCode = StatusCode.ResourceExhausted;
        const string originalMessage = "Too many requests";

        var context = new TestServerCallContext();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        Assert.Contains("Internal server error", exception.Status.Detail);
        
        return;

        Task<string> Continuation(
            string req,
            ServerCallContext ctx
        )
        {
            throw new RpcException(new Status(originalCode, originalMessage));
        }
    }

    [Fact]
    public async Task UnaryServerHandler_WithUnexpectedException_ThrowsRpcExceptionWithInternalCode()
    {
        // Arrange
        const string request = "request";
        const string exceptionMessage = "Unexpected error";

        var context = new TestServerCallContext();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        Assert.Contains("Unexpected server error", exception.Status.Detail);
        
        return;

        Task<string> Continuation(
            string req,
            ServerCallContext ctx
        )
        {
            throw new InvalidOperationException(exceptionMessage);
        }
    }
}
