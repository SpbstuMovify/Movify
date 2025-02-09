using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using JetBrains.Annotations;

using MediaService.Dtos.Content;
using MediaService.FileProcessing;
using MediaService.Grpc.Clients;

using Moq;

namespace MediaService.Tests.Grpc.Clients;

[TestSubject(typeof(ContentGrpcClient))]
public class ContentGrpcClientTest
{
    private readonly Mock<Movify.ContentService.ContentServiceClient> _contentServiceClientMock;
    private readonly ContentGrpcClient _client; // Тестируемый класс

    public ContentGrpcClientTest()
    {
        _contentServiceClientMock = new Mock<Movify.ContentService.ContentServiceClient>();
        _client = new ContentGrpcClient(_contentServiceClientMock.Object);
    }

    [Theory]
    [InlineData("", "http://example.com/img.png", "ContentId cannot be null or empty")]
    [InlineData(" ", "http://example.com/img.png", "ContentId cannot be null or empty")]
    [InlineData("ABC123", "", "Url cannot be null or empty")]
    [InlineData("ABC123", " ", "Url cannot be null or empty")]
    public async Task SetContentImageUrl_WithInvalidData_ThrowsArgumentException(
        string contentId,
        string url,
        string expectedMessageStart
    )
    {
        // Arrange
        var dto = new ContentImageUrlDto(contentId, url);

        // Act
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _client.SetContentImageUrl(dto));

        // Assert
        Assert.StartsWith(expectedMessageStart, ex.Message);
    }

    [Fact]
    public async Task SetContentImageUrl_WithValidData_CallsGrpcMethod()
    {
        // Arrange
        var dto = new ContentImageUrlDto("contentXYZ", "http://example.com/img.png");

        _contentServiceClientMock
            .Setup(
                client => client.SetContentImageUrlAsync(
                    It.IsAny<Movify.SetContentImageUrlRequest>(),
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
        await _client.SetContentImageUrl(dto);

        // Assert
        _contentServiceClientMock.Verify(
            x => x.SetContentImageUrlAsync(
                It.Is<Movify.SetContentImageUrlRequest>(
                    r =>
                        r.ContentId == dto.ContentId &&
                        r.Url == dto.Url
                ),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task SetContentImageUrl_WithGrpcFailure_ThrowsRpcException()
    {
        // Arrange
        var dto = new ContentImageUrlDto("content123", "http://example.com/img.png");
        var expectedStatusCode = StatusCode.Internal;
        var expectedDetail = "some grpc error";

        _contentServiceClientMock
            .Setup(
                client => client.SetContentImageUrlAsync(
                    It.IsAny<Movify.SetContentImageUrlRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Throws(new RpcException(new Status(expectedStatusCode, expectedDetail)));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _client.SetContentImageUrl(dto));
        Assert.Equal(expectedStatusCode, ex.StatusCode);
        Assert.Equal(expectedDetail, ex.Status.Detail);
    }

    [Theory]
    [InlineData("", "EpisodeId cannot be null or empty")]
    [InlineData(" ", "EpisodeId cannot be null or empty")]
    public async Task SetEpisodeVideoUrl_WithInvalidEpisodeId_ThrowsArgumentException(
        string invalidEpisodeId,
        string expectedMessageStart
    )
    {
        // Arrange
        var dto = new EpisodeVideoUrlDto(
            EpisodeId: invalidEpisodeId,
            Url: "http://example.com/video.mp4",
            Status: FileStatus.InProgress
        );

        // Act
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _client.SetEpisodeVideoUrl(dto));

        // Assert
        Assert.StartsWith(expectedMessageStart, ex.Message);
    }

    [Theory]
    [InlineData(FileStatus.NotUploaded, "NOT_UPLOADED")]
    [InlineData(FileStatus.InProgress, "IN_PROGRESS")]
    [InlineData(FileStatus.Error, "ERROR")]
    [InlineData(FileStatus.Uploaded, "UPLOADED")]
    public async Task SetEpisodeVideoUrl_WithValidData_CallsGrpcMethodWithCorrectStatus(
        FileStatus status,
        string expectedStatus
    )
    {
        // Arrange
        var dto = new EpisodeVideoUrlDto(
            EpisodeId: "episodeABC",
            Url: "http://example.com/video.mp4",
            Status: status
        );

        _contentServiceClientMock
            .Setup(
                client => client.SetEpisodeVideoUrlAsync(
                    It.IsAny<Movify.SetEpisodeVideoUrlRequest>(),
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
        await _client.SetEpisodeVideoUrl(dto);

        // Assert
        _contentServiceClientMock.Verify(
            x => x.SetEpisodeVideoUrlAsync(
                It.Is<Movify.SetEpisodeVideoUrlRequest>(
                    r =>
                        r.EpisodeId == dto.EpisodeId &&
                        r.Url == dto.Url &&
                        r.Status == expectedStatus
                ),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task SetEpisodeVideoUrl_WithGrpcFailure_ThrowsRpcException()
    {
        // Arrange
        var dto = new EpisodeVideoUrlDto(
            EpisodeId: "episodeX",
            Url: "http://example.com/videoX.mp4",
            Status: FileStatus.Error
        );

        var expectedStatusCode = StatusCode.Internal;
        var expectedDetail = "grpc call failure";

        _contentServiceClientMock
            .Setup(
                client => client.SetEpisodeVideoUrlAsync(
                    It.IsAny<Movify.SetEpisodeVideoUrlRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Throws(new RpcException(new Status(expectedStatusCode, expectedDetail)));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _client.SetEpisodeVideoUrl(dto));
        Assert.Equal(expectedStatusCode, ex.StatusCode);
        Assert.Equal(expectedDetail, ex.Status.Detail);
    }
}
