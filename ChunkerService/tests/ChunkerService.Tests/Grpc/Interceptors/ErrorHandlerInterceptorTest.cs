using ChunkerService.Grpc.Interceptors;

using Grpc.Core;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Moq;

namespace ChunkerService.Tests.Grpc.Interceptors;

[TestSubject(typeof(ErrorHandlerInterceptor))]
public class ErrorHandlerInterceptorTest
{
    private readonly ErrorHandlerInterceptor _interceptor;

    public ErrorHandlerInterceptorTest()
    {
        var loggerMock = new Mock<ILogger<ErrorHandlerInterceptor>>();
        _interceptor = new ErrorHandlerInterceptor(loggerMock.Object);
    }

    [Fact]
    public async Task UnaryServerHandler_WhenContinuationSucceeds_ReturnsResponse()
    {
        // Arrange
        var request = new object();
        var context = new TestServerCallContext();

        // Act
        var result = await _interceptor.UnaryServerHandler(request, context, Continuation);

        // Assert
        Assert.Equal("Success", result);
        
        return;

        async Task<string> Continuation(
            object req,
            ServerCallContext ctx
        )
        {
            return await Task.FromResult("Success");
        }
    }

    [Fact]
    public async Task UnaryServerHandler_WhenContinuationThrowsRpcExceptionInvalidArgument_LogsWarningAndRethrows()
    {
        // Arrange
        var request = new object();
        var context = new TestServerCallContext();
        const string originalMessage = "Invalid argument error message";

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(
            () => _interceptor.UnaryServerHandler(request, context, Continuation)
        );
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains(originalMessage, ex.Status.Detail);
        
        return;

        Task<string> Continuation(
            object req,
            ServerCallContext ctx
        )
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, originalMessage));
        }
    }

    [Fact]
    public async Task UnaryServerHandler_WhenContinuationThrowsException_LogsErrorAndRethrowsAsInternalError()
    {
        // Arrange
        var request = new object();
        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(
            () => _interceptor.UnaryServerHandler(request, context, Continuation)
        );
        Assert.Equal(StatusCode.Internal, ex.StatusCode);
        Assert.Equal("Unexpected server error", ex.Status.Detail);
        
        return;

        Task<string> Continuation(
            object req,
            ServerCallContext ctx
        )
        {
            throw new Exception("Unexpected error");
        }
    }
}
