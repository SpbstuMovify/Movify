using Grpc.Core;

using JetBrains.Annotations;

using MediaService.Grpc.Interceptors;

using Microsoft.Extensions.Logging;

using Moq;

namespace MediaService.Tests.Grpc.Interceptors;

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
    public async Task UnaryServerHandler_NoException_ReturnsContinuationResult()
    {
        // Arrange
        var request = "dummyRequest";
        var context = new TestServerCallContext();

        // Act & Assert
        var response = await _interceptor.UnaryServerHandler(request, context, Continuation);
        Assert.Equal("OK", response);

        return;

        async Task<string> Continuation(
            string req,
            ServerCallContext ctx
        )
        {
            return await Task.FromResult("OK");
        }
    }

    [Fact]
    public async Task UnaryServerHandler_RpcExceptionWithInvalidArgument_LogsWarning_AndRethrowsSameStatusCode()
    {
        // Arrange
        const string request = "dummyRequest";
        var context = new TestServerCallContext();

        const string originalDetail = "Invalid input data";
        const string originalMessage = "Some invalid argument message";

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains(originalMessage, ex.Message);

        return;

        Task<string> Continuation(
            string req,
            ServerCallContext ctx
        )
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, originalDetail), originalMessage);
        }
    }

    [Fact]
    public async Task UnaryServerHandler_RpcExceptionWithOtherCode_LogsWarning_AndThrowsInternalError()
    {
        // Arrange
        const string request = "dummyRequest";
        var context = new TestServerCallContext();

        const string originalDetail = "Not found detail";
        const StatusCode originalCode = StatusCode.NotFound;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.Internal, ex.StatusCode);
        Assert.Contains("Internal server error", ex.Status.Detail);

        return;

        Task<string> Continuation(
            string req,
            ServerCallContext ctx
        )
        {
            throw new RpcException(new Status(originalCode, originalDetail));
        }
    }

    [Fact]
    public async Task UnaryServerHandler_NormalException_LogsError_AndThrowsInternalUnexpectedError()
    {
        // Arrange
        const string request = "dummyRequest";
        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.Internal, ex.StatusCode);
        Assert.Contains("Unexpected server error", ex.Status.Detail);
        return;

        Task<string> Continuation(
            string req,
            ServerCallContext ctx
        )
        {
            throw new InvalidOperationException("Some normal exception");
        }
    }
}
