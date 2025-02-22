using ChunkerService.Grpc.Interceptors;

using Grpc.Core;

using JetBrains.Annotations;

namespace ChunkerService.Tests.Grpc.Interceptors;

[TestSubject(typeof(ValidationInterceptor))]
public class ValidationInterceptorTest
{
    private readonly ValidationInterceptor _interceptor = new();

    [Fact]
    public async Task ProcessVideoRequest_WithValidData_PassesValidation()
    {
        // Arrange: все поля заданы корректно
        var request = new Movify.ProcessVideoRequest
        {
            BaseUrl = "http://example.com",
            BucketName = "validBucket",
            Key = "content123/episode456"
        };

        var context = new TestServerCallContext();

        // Act
        var response = await _interceptor.UnaryServerHandler(request, context, Continuation);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("OK", response);

        return;

        async Task<string> Continuation(
            Movify.ProcessVideoRequest req,
            ServerCallContext ctx
        )
        {
            return await Task.FromResult("OK");
        }
    }

    [Theory]
    [InlineData("", "validBucket", "content/episode", "BaseUrl cannot be null or empty")]
    [InlineData("   ", "validBucket", "content/episode", "BaseUrl cannot be null or empty")]
    public async Task ProcessVideoRequest_WithInvalidBaseUrl_ThrowsRpcException(
        string baseUrl,
        string bucketName,
        string key,
        string expectedError
    )
    {
        // Arrange: BaseUrl не задан или содержит только пробелы
        var request = new Movify.ProcessVideoRequest
        {
            BaseUrl = baseUrl,
            BucketName = bucketName,
            Key = key
        };

        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains(expectedError, ex.Status.Detail);

        return;

        Task<string> Continuation(
            Movify.ProcessVideoRequest req,
            ServerCallContext ctx
        )
        {
            return Task.FromResult("fake");
        }
    }

    [Theory]
    [InlineData("http://example.com", "", "content/episode", "BucketName cannot be null or empty")]
    [InlineData("http://example.com", "   ", "content/episode", "BucketName cannot be null or empty")]
    public async Task ProcessVideoRequest_WithInvalidBucketName_ThrowsRpcException(
        string baseUrl,
        string bucketName,
        string key,
        string expectedError
    )
    {
        // Arrange: BucketName не задан или состоит только из пробелов
        var request = new Movify.ProcessVideoRequest
        {
            BaseUrl = baseUrl,
            BucketName = bucketName,
            Key = key
        };

        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains(expectedError, ex.Status.Detail);

        return;

        Task<string> Continuation(
            Movify.ProcessVideoRequest req,
            ServerCallContext ctx
        )
        {
            return Task.FromResult("fake");
        }
    }

    [Theory]
    [InlineData("http://example.com", "validBucket", "", "Key cannot be null or empty")]
    [InlineData("http://example.com", "validBucket", "   ", "Key cannot be null or empty")]
    public async Task ProcessVideoRequest_WithInvalidKey_ThrowsRpcException(
        string baseUrl,
        string bucketName,
        string key,
        string expectedError
    )
    {
        // Arrange: Key не задан или содержит только пробелы
        var request = new Movify.ProcessVideoRequest
        {
            BaseUrl = baseUrl,
            BucketName = bucketName,
            Key = key
        };

        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains(expectedError, ex.Status.Detail);

        return;

        Task<string> Continuation(
            Movify.ProcessVideoRequest req,
            ServerCallContext ctx
        )
        {
            return Task.FromResult("fake");
        }
    }

    [Fact]
    public async Task UnsupportedRequest_ThrowsRpcException()
    {
        // Arrange: передаётся неподдерживаемый тип запроса
        var request = new object();
        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("Unsupported operation", ex.Status.Detail);

        return;

        Task<string> Continuation(
            object req,
            ServerCallContext ctx
        )
        {
            return Task.FromResult("fake");
        }
    }
}
