using JetBrains.Annotations;

using MediaService.Dtos.Chunker;
using MediaService.Dtos.Content;
using MediaService.Dtos.S3;
using MediaService.FileProcessing;
using MediaService.FileProcessing.FileProcessors;
using MediaService.Grpc.Clients;
using MediaService.Repositories;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace MediaService.Tests.FileProcessing.FileProcessors;

[TestSubject(typeof(EpisodeVideoFileProcessor))]
public class EpisodeVideoFileProcessorTest
{
    private readonly Mock<IBucketRepository> _bucketRepositoryMock;
    private readonly Mock<IContentGrpcClient> _contentGrpcClientMock;
    private readonly Mock<IChunckerGrpcClient> _chunkerGrpcClientMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly EpisodeVideoFileProcessor _episodeVideoFileProcessor;

    public EpisodeVideoFileProcessorTest()
    {
        _bucketRepositoryMock = new Mock<IBucketRepository>();
        _contentGrpcClientMock = new Mock<IContentGrpcClient>();
        _chunkerGrpcClientMock = new Mock<IChunckerGrpcClient>();
        _serviceScopeMock = new Mock<IServiceScope>();

        var loggerMock = new Mock<ILogger<EpisodeVideoFileProcessor>>();
        var serviceProviderMock = new Mock<IServiceProvider>();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IBucketRepository)))
            .Returns(_bucketRepositoryMock.Object);

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IContentGrpcClient)))
            .Returns(_contentGrpcClientMock.Object);

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IChunckerGrpcClient)))
            .Returns(_chunkerGrpcClientMock.Object);

        _serviceScopeMock
            .Setup(s => s.ServiceProvider)
            .Returns(serviceProviderMock.Object);

        _episodeVideoFileProcessor = new EpisodeVideoFileProcessor(loggerMock.Object);
    }
    
    [Fact]
    public void Destination_WithEpisodeVideoUrlDestination()
    {
        Assert.Equal(FileDestination.EpisodeVideoUrl, _episodeVideoFileProcessor.Destination);
    }

    [Fact]
    public async Task ProcessAsync_WithIsVideoProcessingFalse()
    {
        // Arrange
        var requestKey = "content123/episodeId123/video.mp4";
        var testContent = new MemoryStream([1, 2, 3, 4]);
        var fileData = new FileData(
            Content: testContent,
            ContentType: "video/mp4",
            FileName: "video.mp4"
        );

        var request = new FileProcessorRequest(
            File: fileData,
            BucketName: "test-bucket",
            Key: requestKey,
            BaseUrl: "http://example.com",
            IsVideoProcNecessary: false
        );

        var expectedS3Object = new S3ObjectDto(
            BucketName: "test-bucket",
            PresignedUrl: "https://presigned-url"
        );

        _bucketRepositoryMock
            .Setup(
                repo => repo.UploadFileAsync(
                    It.IsAny<FileData>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(expectedS3Object);

        // Act
        await _episodeVideoFileProcessor.ProcessAsync(request, _serviceScopeMock.Object);

        // Assert
        _contentGrpcClientMock.Verify(
            c => c.SetEpisodeVideoUrl(
                It.Is<EpisodeVideoUrlDto>(
                    dto =>
                        dto.EpisodeId == "episodeId123" &&
                        dto.Url == "" &&
                        dto.Status == FileStatus.InProgress
                )
            ),
            Times.Once
        );

        _bucketRepositoryMock.Verify(
            repo => repo.UploadFileAsync(
                fileData,
                "test-bucket",
                requestKey
            ),
            Times.Once
        );

        _contentGrpcClientMock.Verify(
            c => c.SetEpisodeVideoUrl(
                It.Is<EpisodeVideoUrlDto>(
                    dto =>
                        dto.EpisodeId == "episodeId123" &&
                        dto.Url == "http://example.com/content123/episodeId123/video.mp4" &&
                        dto.Status == FileStatus.Uploaded
                )
            ),
            Times.Once
        );

        _chunkerGrpcClientMock.Verify(
            c => c.ProcessVideo(It.IsAny<ProcessVideoDto>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ProcessAsync_WithIsVideoProcessingTrue()
    {
        // Arrange
        var requestKey = "contentXYZ/episodeABC/video.mov";
        var testContent = new MemoryStream([11, 22, 33, 44]);
        var fileData = new FileData(
            Content: testContent,
            ContentType: "video/quicktime",
            FileName: "video.mov"
        );

        var request = new FileProcessorRequest(
            File: fileData,
            BucketName: "bucket123",
            Key: requestKey,
            BaseUrl: "https://cdn.example.com",
            IsVideoProcNecessary: true
        );

        var expectedS3Object = new S3ObjectDto(
            BucketName: "bucket123",
            PresignedUrl: "https://some-presigned-url"
        );

        _bucketRepositoryMock
            .Setup(
                repo => repo.UploadFileAsync(
                    It.IsAny<FileData>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(expectedS3Object);

        // Act
        await _episodeVideoFileProcessor.ProcessAsync(request, _serviceScopeMock.Object);

        // Assert
        _contentGrpcClientMock.Verify(
            c => c.SetEpisodeVideoUrl(
                It.Is<EpisodeVideoUrlDto>(
                    dto =>
                        dto.EpisodeId == "episodeABC" &&
                        dto.Url == "" &&
                        dto.Status == FileStatus.InProgress
                )
            ),
            Times.Once
        );

        _bucketRepositoryMock.Verify(
            repo => repo.UploadFileAsync(
                fileData,
                "bucket123",
                requestKey
            ),
            Times.Once
        );

        _contentGrpcClientMock.Verify(
            c => c.SetEpisodeVideoUrl(
                It.Is<EpisodeVideoUrlDto>(
                    dto =>
                        dto.EpisodeId == "episodeABC" &&
                        dto.Url == "https://cdn.example.com/contentXYZ/episodeABC/video.mov" &&
                        dto.Status == FileStatus.Uploaded
                )
            ),
            Times.Once
        );

        _chunkerGrpcClientMock.Verify(
            c => c.ProcessVideo(
                It.Is<ProcessVideoDto>(
                    p =>
                        p.BucketName == "bucket123" &&
                        p.Key == requestKey &&
                        p.BaseUrl == "https://cdn.example.com"
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task ProcessAsync_WhenUploadThrows_Rethrow()
    {
        // Arrange
        var requestKey = "cnt/ep123/video.avi";
        var testContent = new MemoryStream([5, 5, 5, 5]);
        var fileData = new FileData(
            Content: testContent,
            ContentType: "video/avi",
            FileName: "video.avi"
        );

        var request = new FileProcessorRequest(
            File: fileData,
            BucketName: "bucketError",
            Key: requestKey,
            BaseUrl: "http://videos.test",
            IsVideoProcNecessary: false
        );

        _bucketRepositoryMock
            .Setup(
                repo => repo.UploadFileAsync(
                    It.IsAny<FileData>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ThrowsAsync(new InvalidOperationException("Some upload error"));

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _episodeVideoFileProcessor.ProcessAsync(request, _serviceScopeMock.Object));

        // Assert
        Assert.Equal("Some upload error", exception.Message);

        _contentGrpcClientMock.Verify(
            c => c.SetEpisodeVideoUrl(
                It.Is<EpisodeVideoUrlDto>(
                    dto =>
                        dto.EpisodeId == "ep123" &&
                        dto.Url == "" &&
                        dto.Status == FileStatus.InProgress
                )
            ),
            Times.Once
        );

        _contentGrpcClientMock.Verify(
            c => c.SetEpisodeVideoUrl(
                It.Is<EpisodeVideoUrlDto>(
                    dto =>
                        dto.EpisodeId == "ep123" &&
                        dto.Url == "" &&
                        dto.Status == FileStatus.Error
                )
            ),
            Times.Once
        );
    }
}
