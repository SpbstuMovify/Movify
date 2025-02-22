using JetBrains.Annotations;

using MediaService.Dtos.Chunker;
using MediaService.Dtos.Content;
using MediaService.FileProcessing;
using MediaService.Grpc.Clients;
using MediaService.Services;

using Moq;

namespace MediaService.Tests.Services;

[TestSubject(typeof(ChunkerCallbackService))]
public class ChunkerCallbackServiceTest
{
    private readonly Mock<IContentGrpcClient> _contentGrpcClientMock;
    private readonly ChunkerCallbackService _service;

    public ChunkerCallbackServiceTest()
    {
        _contentGrpcClientMock = new Mock<IContentGrpcClient>();
        _service = new ChunkerCallbackService(_contentGrpcClientMock.Object);
    }

    [Fact]
    public async Task OnSuccess_WithValidKey_SetsEpisodeVideoUrlAsUploaded()
    {
        // Arrange
        // Допустим, Key имеет вид content123/episodeABC/file.mp4
        // тогда episodeId = episodeABC
        var successDto = new ProcessVideoCallbackSuccessDto("content123/episodeABC/file.mp4", "http://example.com");

        // Act
        await _service.OnSuccess(successDto);

        // Assert
        _contentGrpcClientMock.Verify(
            x => x.SetEpisodeVideoUrl(
                It.Is<EpisodeVideoUrlDto>(
                    dto =>
                        dto.EpisodeId == "episodeABC"
                        && dto.Url == "http://example.com/content123/episodeABC/file.mp4"
                        && dto.Status == FileStatus.Uploaded
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task OnSuccess_WithMissingSecondElement_SetsEpisodeIdEmpty()
    {
        // Arrange
        var successDto = new ProcessVideoCallbackSuccessDto("contentOnly", "http://example.com");

        // Act
        await _service.OnSuccess(successDto);

        // Assert
        _contentGrpcClientMock.Verify(
            x => x.SetEpisodeVideoUrl(
                It.Is<EpisodeVideoUrlDto>(
                    dto =>
                        dto.EpisodeId == "" &&
                        dto.Url == "http://example.com/contentOnly" &&
                        dto.Status == FileStatus.Uploaded
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task OnFailed_WithValidKey_SetsEpisodeVideoUrlAsError()
    {
        // Arrange
        // Key = content123/episodeABC/file.mp4 => episodeId = "episodeABC"
        var failedDto = new ProcessVideoCallbackFailedDto("content123/episodeABC/file.mp4");

        // Act
        await _service.OnFailed(failedDto);

        // Assert
        _contentGrpcClientMock.Verify(
            x => x.SetEpisodeVideoUrl(
                It.Is<EpisodeVideoUrlDto>(
                    dto =>
                        dto.EpisodeId == "episodeABC" &&
                        dto.Url == "" &&
                        dto.Status == FileStatus.Error
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task OnFailed_WithMissingSecondElement_SetsEpisodeIdEmpty()
    {
        // Arrange
        var failedDto = new ProcessVideoCallbackFailedDto("justOnePart");

        // Act
        await _service.OnFailed(failedDto);

        // Assert
        _contentGrpcClientMock.Verify(
            x => x.SetEpisodeVideoUrl(
                It.Is<EpisodeVideoUrlDto>(
                    dto =>
                        dto.EpisodeId == "" &&
                        dto.Url == "" &&
                        dto.Status == FileStatus.Error
                )
            ),
            Times.Once
        );
    }
}
