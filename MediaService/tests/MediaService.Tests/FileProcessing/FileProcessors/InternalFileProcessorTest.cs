using JetBrains.Annotations;

using MediaService.Dtos.S3;
using MediaService.FileProcessing;
using MediaService.FileProcessing.FileProcessors;
using MediaService.Repositories;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace MediaService.Tests.FileProcessing.FileProcessors;

[TestSubject(typeof(InternalFileProcessor))]
public class InternalFileProcessorTest
{
    private readonly Mock<IBucketRepository> _bucketRepositoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly InternalFileProcessor _internalFileProcessor;

    public InternalFileProcessorTest()
    {
        Mock<ILogger<InternalFileProcessor>> loggerMock = new Mock<ILogger<InternalFileProcessor>>();
        Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();

        _bucketRepositoryMock = new Mock<IBucketRepository>();
        _serviceScopeMock = new Mock<IServiceScope>();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IBucketRepository)))
            .Returns(_bucketRepositoryMock.Object);

        _serviceScopeMock
            .Setup(s => s.ServiceProvider)
            .Returns(serviceProviderMock.Object);

        _internalFileProcessor = new InternalFileProcessor(loggerMock.Object);
    }
    
    [Fact]
    public void Destination_WithInternalDestination()
    {
        Assert.Equal(FileDestination.Internal, _internalFileProcessor.Destination);
    }

    [Fact]
    public async Task ProcessAsync_WithValidRequest()
    {
        var testContent = new MemoryStream([1, 2, 3, 4]);

        var fileData = new FileData(
            Content: testContent,
            ContentType: "application/octet-stream",
            FileName: "test.txt"
        );

        var request = new FileProcessorRequest(
            File: fileData,
            BucketName: "test-bucket",
            Key: "test-key",
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
        await _internalFileProcessor.ProcessAsync(request, _serviceScopeMock.Object);

        // Assert
        _bucketRepositoryMock.Verify(
            repo => repo.UploadFileAsync(
                fileData,
                "test-bucket",
                "test-key"
            ),
            Times.Once
        );
    }
}
