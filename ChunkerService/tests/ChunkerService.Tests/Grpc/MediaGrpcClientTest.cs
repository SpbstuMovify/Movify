using ChunkerService.Dtos.Chunker;
using ChunkerService.Grpc;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using JetBrains.Annotations;

using Moq;

namespace ChunkerService.Tests.Grpc;

[TestSubject(typeof(MediaGrpcClient))]
public class MediaGrpcClientTest
{
    private readonly Mock<Movify.MediaService.MediaServiceClient> _mediaServiceClientMock;
    private readonly MediaGrpcClient _client; // Тестируемый класс

    public MediaGrpcClientTest()
    {
        _mediaServiceClientMock = new Mock<Movify.MediaService.MediaServiceClient>();
        _client = new MediaGrpcClient(_mediaServiceClientMock.Object);
    }

    [Theory]
    [InlineData("", "myKey", "http://example.com", "BucketName cannot be null or empty")]
    [InlineData(" ", "myKey", "http://example.com", "BucketName cannot be null or empty")]
    [InlineData("myBucket", "", "http://example.com", "Key cannot be null or empty")]
    [InlineData("myBucket", " ", "http://example.com", "Key cannot be null or empty")]
    public async Task ProcessVideoCallback_WithInvalidInput_ThrowsArgumentException(
        string bucketName,
        string key,
        string baseUrl,
        string expectedMessageStart
    )
    {
        // Arrange
        // В данном тесте значение Error можно задать произвольным, так как валидация проходит только для BucketName и Key
        var dto = new ProcessVideoCallbackDto(bucketName, key, baseUrl, "some error");

        // Act
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _client.ProcessVideoCallback(dto));

        // Assert
        Assert.StartsWith(expectedMessageStart, ex.Message);
    }

    [Fact]
    public async Task ProcessVideoCallback_WithValidDto_CallsGrpcProcessVideoCallbackAsync()
    {
        // Arrange
        var dto = new ProcessVideoCallbackDto("myBucket", "myKey", "http://example.com", "some error");

        _mediaServiceClientMock
            .Setup(
                x => x.ProcessVideoCallbackAsync(
                    It.IsAny<Movify.ProcessVideoCallbackRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(
                new AsyncUnaryCall<Empty>(
                    Task.FromResult(new Empty()),
                    Task.FromResult(new Metadata()),
                    () => new Status(StatusCode.OK, string.Empty),
                    () => [],
                    () => { }
                )
            );

        // Act
        await _client.ProcessVideoCallback(dto);

        // Assert
        _mediaServiceClientMock.Verify(
            x => x.ProcessVideoCallbackAsync(
                It.Is<Movify.ProcessVideoCallbackRequest>(
                    r =>
                        r.BucketName == dto.BucketName &&
                        r.Key == dto.Key &&
                        r.BaseUrl == dto.BaseUrl &&
                        r.Error == dto.Error
                ),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task ProcessVideoCallback_WithGrpcFailure_ThrowsRpcException()
    {
        // Arrange
        var dto = new ProcessVideoCallbackDto("myBucket", "myKey", "http://example.com", "some error");

        const StatusCode expectedStatusCode = StatusCode.Internal;
        const string expectedDetail = "Internal error";

        _mediaServiceClientMock
            .Setup(
                x => x.ProcessVideoCallbackAsync(
                    It.IsAny<Movify.ProcessVideoCallbackRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Throws(new RpcException(new Status(expectedStatusCode, expectedDetail)));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _client.ProcessVideoCallback(dto));
        Assert.Equal(expectedStatusCode, ex.StatusCode);
        Assert.Equal(expectedDetail, ex.Status.Detail);

        _mediaServiceClientMock.Verify(
            x => x.ProcessVideoCallbackAsync(
                It.Is<Movify.ProcessVideoCallbackRequest>(
                    r =>
                        r.BucketName == dto.BucketName &&
                        r.Key == dto.Key &&
                        r.BaseUrl == dto.BaseUrl &&
                        r.Error == dto.Error
                ),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}
