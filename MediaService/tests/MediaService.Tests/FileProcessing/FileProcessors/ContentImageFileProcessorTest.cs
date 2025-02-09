using JetBrains.Annotations;

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

[TestSubject(typeof(ContentImageFileProcessor))]
public class ContentImageFileProcessorTest
{
    private readonly Mock<IBucketRepository> _bucketRepositoryMock;
    private readonly Mock<IContentGrpcClient> _contentGrpcClientMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly ContentImageFileProcessor _contentImageFileProcessor;

    public ContentImageFileProcessorTest()
    {
        _bucketRepositoryMock = new Mock<IBucketRepository>();
        _contentGrpcClientMock = new Mock<IContentGrpcClient>();
        _serviceScopeMock = new Mock<IServiceScope>();

        var loggerMock = new Mock<ILogger<ContentImageFileProcessor>>();
        var serviceProviderMock = new Mock<IServiceProvider>();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IBucketRepository)))
            .Returns(_bucketRepositoryMock.Object);

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IContentGrpcClient)))
            .Returns(_contentGrpcClientMock.Object);

        _serviceScopeMock
            .Setup(s => s.ServiceProvider)
            .Returns(serviceProviderMock.Object);

        // Создаём экземпляр тестируемого класса
        _contentImageFileProcessor = new ContentImageFileProcessor(loggerMock.Object);
    }

    [Fact]
    public void Destination_WithContentImageUrlDestination()
    {
        Assert.Equal(FileDestination.ContentImageUrl, _contentImageFileProcessor.Destination);
    }

    [Fact]
    public async Task ProcessAsync_WithValidRequest()
    {
        var testContent = new MemoryStream([1, 2, 3, 4]);
        var fileData = new FileData(
            Content: testContent,
            ContentType: "image/png",
            FileName: "test.png"
        );

        var request = new FileProcessorRequest(
            File: fileData,
            BucketName: "test-bucket",
            Key: "1234/image.png",
            BaseUrl: "http://example.com",
            IsVideoProcNecessary: false
        );

        var expectedS3Object = new S3ObjectDto(
            BucketName: "test-bucket",
            PresignedUrl: "https://test-url"
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
        await _contentImageFileProcessor.ProcessAsync(request, _serviceScopeMock.Object);

        // Assert
        _bucketRepositoryMock.Verify(
            repo => repo.UploadFileAsync(
                fileData,
                "test-bucket",
                "1234/image.png"
            ),
            Times.Once
        );

        _contentGrpcClientMock.Verify(
            client => client.SetContentImageUrl(
                It.Is<ContentImageUrlDto>(dto => dto.ContentId == "1234" && dto.Url == "http://example.com/1234/image.png")
            ),
            Times.Once
        );
    }
}
