using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using JetBrains.Annotations;

using MediaService.Dtos.Chunker;
using MediaService.Grpc.Clients;

using Moq;

namespace MediaService.Tests.Grpc.Clients;

[TestSubject(typeof(ChunkerGrpcClient))]
public class ChunkerGrpcClientTest
{
    private readonly Mock<Movify.ChunkerService.ChunkerServiceClient> _chunkerClientMock;
    private readonly ChunkerGrpcClient _client; // Тестируемый класс

    public ChunkerGrpcClientTest()
    {
        _chunkerClientMock = new Mock<Movify.ChunkerService.ChunkerServiceClient>();
        _client = new ChunkerGrpcClient(_chunkerClientMock.Object);
    }
    
    [Theory]
    [InlineData("", "myKey", "http://example.com", "BucketName cannot be null or empty")]
    [InlineData(" ", "myKey", "http://example.com", "BucketName cannot be null or empty")]
    [InlineData("myBucket", "", "http://example.com", "Key cannot be null or empty")]
    [InlineData("myBucket", " ", "http://example.com", "Key cannot be null or empty")]
    [InlineData("myBucket", "myKey", "", "BaseUrl cannot be null or empty")]
    [InlineData("myBucket", "myKey", " ", "BaseUrl cannot be null or empty")]
    public async Task ProcessVideo_WithInvalidInput_ThrowsArgumentException(
        string bucketName,
        string key,
        string baseUrl,
        string expectedMessageStart
    )
    {
        // Arrange
        var dto = new ProcessVideoDto(bucketName, key, baseUrl);

        // Act
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _client.ProcessVideo(dto));

        // Assert
        Assert.StartsWith(expectedMessageStart, ex.Message);
    }

    // 2. Успешный сценарий
    [Fact]
    public async Task ProcessVideo_WithValidDto_CallsGrpcProcessVideoAsync()
    {
        // Arrange
        var dto = new ProcessVideoDto("myBucket", "myKey", "http://example.com");
        
        _chunkerClientMock
            .Setup(
                x => x.ProcessVideoAsync(
                    It.IsAny<Movify.ProcessVideoRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(
                new AsyncUnaryCall<Empty>(
                    Task.FromResult(new Empty()),
                    Task.FromResult(new Metadata()),
                    () => new Status(StatusCode.OK, ""),
                    () => new Metadata(),
                    () => { }
                )
            );

        // Act
        await _client.ProcessVideo(dto);

        // Assert
        _chunkerClientMock.Verify(
            x => x.ProcessVideoAsync(
                It.Is<Movify.ProcessVideoRequest>(
                    r =>
                        r.BucketName == dto.BucketName &&
                        r.Key == dto.Key &&
                        r.BaseUrl == dto.BaseUrl
                ),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
    
    [Fact]
    public async Task ProcessVideo_WithGrpcFailure_ThrowsRpcException()
    {
        // Arrange
        var dto = new ProcessVideoDto("validBucket", "validKey", "http://example.com");

        var expectedStatusCode = StatusCode.Internal;
        var expectedDetail = "Some internal error";

        _chunkerClientMock
            .Setup(
                client => client.ProcessVideoAsync(
                    It.IsAny<Movify.ProcessVideoRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Throws(new RpcException(new Status(expectedStatusCode, expectedDetail)));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _client.ProcessVideo(dto));

        Assert.Equal(expectedStatusCode, ex.StatusCode);
        Assert.Equal(expectedDetail, ex.Status.Detail);

        _chunkerClientMock.Verify(
            client => client.ProcessVideoAsync(
                It.Is<Movify.ProcessVideoRequest>(
                    r =>
                        r.BucketName == dto.BucketName &&
                        r.Key == dto.Key &&
                        r.BaseUrl == dto.BaseUrl
                ),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}
