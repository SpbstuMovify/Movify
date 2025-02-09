using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using JetBrains.Annotations;

using MediaService.Dtos.Chunker;
using MediaService.Grpc;
using MediaService.Services;

using Microsoft.Extensions.Logging;

using Moq;

namespace MediaService.Tests.Grpc;

[TestSubject(typeof(MediaGrpcServer))]
public class MediaGrpcServerTest
{
    private readonly Mock<IChunkerCallbackService> _chunkerCallbackServiceMock;
    private readonly MediaGrpcServer _mediaGrpcServer;

    public MediaGrpcServerTest()
    {
        _chunkerCallbackServiceMock = new Mock<IChunkerCallbackService>();

        var loggerMock = new Mock<ILogger<MediaGrpcServer>>();

        _mediaGrpcServer = new MediaGrpcServer(
            loggerMock.Object,
            _chunkerCallbackServiceMock.Object
        );
    }

    [Fact]
    public async Task ProcessVideoCallback_WhenErrorIsEmpty_CallsOnSuccess()
    {
        // Arrange
        var request = new Movify.ProcessVideoCallbackRequest
        {
            Key = "abc/episode123/video.mp4",
            BaseUrl = "http://example.com",
            Error = ""
        };

        // Act
        var response = await _mediaGrpcServer.ProcessVideoCallback(request, null!);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<Empty>(response);
        
        _chunkerCallbackServiceMock.Verify(
            x => x.OnSuccess(
                It.Is<ProcessVideoDtoCallbackSuccessDto>(
                    dto =>
                        dto.Key == request.Key &&
                        dto.BaseUrl == request.BaseUrl
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task ProcessVideoCallback_WhenErrorIsNotEmpty_CallsOnFailedAndLogsWarning()
    {
        // Arrange
        var request = new Movify.ProcessVideoCallbackRequest
        {
            Key = "abc/episode123/video.mp4",
            BaseUrl = "http://example.com",
            Error = "some error"
        };

        // Act
        var response = await _mediaGrpcServer.ProcessVideoCallback(request, null!);

        // Assert
        Assert.NotNull(response);
        Assert.IsType<Empty>(response);
        
        _chunkerCallbackServiceMock.Verify(
            x => x.OnFailed(
                It.Is<ProcessVideoDtoCallbackFailedDto>(
                    dto =>
                        dto.Key == request.Key
                )
            ),
            Times.Once
        );
    }
}
