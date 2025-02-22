using ChunkerService.Dtos.Chunker;
using ChunkerService.Grpc;
using ChunkerService.Hls;
using ChunkerService.Services;

using Google.Protobuf.WellKnownTypes;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Moq;

namespace ChunkerService.Tests.Grpc;

[TestSubject(typeof(ChunkerGrpcServer))]
public class ChunkerGrpcServerTest
{
    private readonly Mock<IChunkerService> _chunkerServiceMock;
    private readonly Mock<IHlsCreator> _hlsCreatorMock;
    private readonly ChunkerGrpcServer _serverUnderTest;

    public ChunkerGrpcServerTest()
    {
        var loggerMock = new Mock<ILogger<ChunkerGrpcServer>>();
        _chunkerServiceMock = new Mock<IChunkerService>();
        _hlsCreatorMock = new Mock<IHlsCreator>();

        _serverUnderTest = new ChunkerGrpcServer(
            loggerMock.Object,
            _chunkerServiceMock.Object,
            _hlsCreatorMock.Object
        );
    }

    [Fact]
    public async Task ProcessVideo_CallsChunkerServiceAndReturnsEmpty()
    {
        // Arrange
        var request = new Movify.ProcessVideoRequest
        {
            BucketName = "test-bucket",
            Key = "test-key",
            BaseUrl = "http://example.com"
        };

        // Act
        var response = await _serverUnderTest.ProcessVideo(request, null!);

        // Assert
        _chunkerServiceMock.Verify(
            cs => cs.ProcessVideo(
                It.Is<ProcessVideoDto>(
                    dto =>
                        dto.BucketName == request.BucketName &&
                        dto.Key == request.Key &&
                        dto.BaseUrl == request.BaseUrl
                )
            ),
            Times.Once
        );

        Assert.IsType<Empty>(response);
    }

    [Fact]
    public async Task CancelVideoProcessing_CallsHlsCreatorCancelTokenAndReturnsEmpty()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var request = new Movify.CancelVideoProcessingRequest
        {
            TokenGuid = guid.ToString()
        };

        // Act
        var response = await _serverUnderTest.CancelVideoProcessing(request, null!);

        // Assert
        _hlsCreatorMock.Verify(h => h.CancelToken(guid), Times.Once);

        Assert.IsType<Empty>(response);
    }
}
