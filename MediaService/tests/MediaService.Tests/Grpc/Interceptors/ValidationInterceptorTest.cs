using Grpc.Core;

using JetBrains.Annotations;

using MediaService.Grpc.Interceptors;

namespace MediaService.Tests.Grpc.Interceptors;

[TestSubject(typeof(ValidationInterceptor))]
public class ValidationInterceptorTest
{
    private readonly ValidationInterceptor _interceptor = new();
    
    [Fact]
    public async Task ProcessVideoCallbackRequest_WithBaseUrlAndNoError_AndValidBucket_AndValidKey_PassesValidation()
    {
        // Arrange
        var request = new Movify.ProcessVideoCallbackRequest
        {
            BaseUrl = "http://example.com",
            Error = "",
            BucketName = "myBucket",
            Key = "content123/episodeXYZ/video.mp4"
        };

        var context = new TestServerCallContext();

        // Act
        var response = await _interceptor.UnaryServerHandler(request, context, Continuation);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response);
        
        return;

        async Task<string> Continuation(Movify.ProcessVideoCallbackRequest req, ServerCallContext ctx)
        {
            return await Task.FromResult("OK");
        }
    }

    [Fact]
    public async Task ProcessVideoCallbackRequest_WithErrorAndNoBaseUrl_AndValidBucket_AndValidKey_PassesValidation()
    {
        // Arrange
        var request = new Movify.ProcessVideoCallbackRequest
        {
            BaseUrl = "",
            Error = "some error",
            BucketName = "validBucket",
            Key = "c123/e456"
        };

        var context = new TestServerCallContext();

        // Act
        var response = await _interceptor.UnaryServerHandler(request, context, Continuation);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response);
        
        return;

        async Task<string> Continuation(Movify.ProcessVideoCallbackRequest req, ServerCallContext ctx)
        {
            return await Task.FromResult("OK");
        }
    }

    [Fact]
    public async Task ProcessVideoCallbackRequest_WhenBothBaseUrlAndErrorEmpty_ThrowsRpcException()
    {
        // Arrange
        var request = new Movify.ProcessVideoCallbackRequest
        {
            BaseUrl = "",
            Error = "",
            BucketName = "someBucket",
            Key = "content/episode"
        };

        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("Exactly one of BaseUrl or Error must be set", ex.Status.Detail);
        Assert.Contains("(BaseUrl='', Error='')", ex.Status.Detail);
        
        return;

        // Локальная функция для imitation
        Task<string> Continuation(Movify.ProcessVideoCallbackRequest req, ServerCallContext ctx)
        {
            return Task.FromResult("fake");
        }
    }

    [Fact]
    public async Task ProcessVideoCallbackRequest_WhenBothBaseUrlAndErrorNotEmpty_ThrowsRpcException()
    {
        // Arrange
        var request = new Movify.ProcessVideoCallbackRequest
        {
            BaseUrl = "http://example.com",
            Error = "some error message",
            BucketName = "someBucket",
            Key = "content/episode"
        };

        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("Exactly one of BaseUrl or Error must be set", ex.Status.Detail);
        
        return;

        Task<string> Continuation(Movify.ProcessVideoCallbackRequest req, ServerCallContext ctx)
        {
            return Task.FromResult("fake");
        }
    }

    [Fact]
    public async Task ProcessVideoCallbackRequest_WhenBucketNameIsEmpty_ThrowsRpcException()
    {
        // Arrange
        var request = new Movify.ProcessVideoCallbackRequest
        {
            BaseUrl = "http://example.com",
            Error = "",
            BucketName = "",
            Key = "content12/episode34"
        };

        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("BucketName cannot be null or empty", ex.Status.Detail);
        
        return;

        Task<string> Continuation(Movify.ProcessVideoCallbackRequest req, ServerCallContext ctx)
        {
            return Task.FromResult("fake");
        }
    }

    [Fact]
    public async Task ProcessVideoCallbackRequest_WhenKeyHasLessThanTwoParts_ThrowsRpcException()
    {
        // Arrange
        var request = new Movify.ProcessVideoCallbackRequest
        {
            BaseUrl = "http://example.com",
            Error = "",
            BucketName = "validBucket",
            Key = "onlyOnePart"
        };

        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("Key 'onlyOnePart' is invalid. Expected format: {content_id}/{episode_id}/...", ex.Status.Detail);
        
        return;

        Task<string> Continuation(Movify.ProcessVideoCallbackRequest req, ServerCallContext ctx)
        {
            return Task.FromResult("fake");
        }
    }
    
    [Fact]
    public async Task UnsupportedRequest_ThrowsRpcException()
    {
        // Arrange
        var request = new object();
        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("Unsupported operation", ex.Status.Detail);
        
        return;

        Task<object> Continuation(object req, ServerCallContext ctx)
        {
            return Task.FromResult(new object());
        }
    }
}
