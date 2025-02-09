using JetBrains.Annotations;

using MediaService.Dtos.FileInfo;
using MediaService.FileProcessing;
using MediaService.FileProcessing.FileProcessors;
using MediaService.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace MediaService.Tests.Services;

[TestSubject(typeof(FileProcessingService))]
public class FileProcessingServiceTest
{
    private readonly Mock<IFileProcessingQueue> _queueMock;
    private readonly Mock<IFileProcessor> _processorMock;
    private readonly Mock<IFileProcessorFactory> _processorFactoryMock;

    private readonly FileProcessingService _serviceUnderTest;

    public FileProcessingServiceTest()
    {
        _queueMock = new Mock<IFileProcessingQueue>();
        _processorMock = new Mock<IFileProcessor>();
        _processorFactoryMock = new Mock<IFileProcessorFactory>();
        
        var loggerMock = new Mock<ILogger<FileProcessingServiceV1>>();
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        var scopeMock = new Mock<IServiceScope>();

        scopeFactoryMock
            .Setup(sf => sf.CreateScope())
            .Returns(scopeMock.Object);

        _processorMock
            .SetupGet(p => p.Destination)
            .Returns(FileDestination.Internal);

        _processorFactoryMock
            .Setup(f => f.GetProcessor(FileDestination.Internal))
            .Returns(_processorMock.Object);

        _serviceUnderTest = new FileProcessingService(
            _queueMock.Object,
            scopeFactoryMock.Object,
            _processorFactoryMock.Object,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_HappyPath_ProcessesFile_AndCleansDirectory()
    {
        // Arrange
        var tempDir = Path.Combine(Directory.GetCurrentDirectory(), "temp", Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var tempFilePath = Path.Combine(tempDir, "testfile.txt");
        await File.WriteAllTextAsync(tempFilePath, "test content");

        var fileTask = new FileProcessingTask(
            new UploadedFileInfoDto(tempFilePath, "text/plain", "testfile.txt"),
            "test-bucket",
            "some/key",
            false,
            FileDestination.Internal,
            "http://example.com"
        );

        _queueMock
            .SetupSequence(q => q.DequeueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileTask)
            .ThrowsAsync(new OperationCanceledException());

        _processorMock
            .Setup(p => p.ProcessAsync(It.IsAny<FileProcessorRequest>(), It.IsAny<IServiceScope>()))
            .Returns(Task.CompletedTask);

        // Act
        using var cts = new CancellationTokenSource(500);
        await _serviceUnderTest.StartAsync(cts.Token);
        await _serviceUnderTest.StopAsync(CancellationToken.None);

        // Assert
        _processorFactoryMock.Verify(
            f => f.GetProcessor(It.IsAny<FileDestination>()),
            Times.Once
        );

        _processorMock.Verify(
            p => p.ProcessAsync(
                It.Is<FileProcessorRequest>(
                    r =>
                        r.BucketName == "test-bucket"
                        && r.File.FileName == "testfile.txt"
                        && r.Key == "some/key"
                ),
                It.IsAny<IServiceScope>()
            ),
            Times.Once
        );

        Assert.False(Directory.Exists(tempDir), "The temporary directory must be deleted");
    }

    [Fact]
    public async Task ExecuteAsync_WhenProcessTaskThrows_LogsErrorAndContinues()
    {
        // Arrange
        var tempDir = Path.Combine(Directory.GetCurrentDirectory(), "temp", Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var tempFilePath = Path.Combine(tempDir, "testfile.txt");
        await File.WriteAllTextAsync(tempFilePath, "test content");

        var fileTask = new FileProcessingTask(
            new UploadedFileInfoDto(tempFilePath, "text/plain", "file.txt"),
            "bucketName",
            "some/key",
            false,
            FileDestination.Internal,
            "http://example.com"
        );

        _queueMock
            .SetupSequence(q => q.DequeueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileTask)
            .ThrowsAsync(new OperationCanceledException());

        _processorMock
            .Setup(
                p => p.ProcessAsync(
                    It.IsAny<FileProcessorRequest>(),
                    It.IsAny<IServiceScope>()
                )
            )
            .ThrowsAsync(new InvalidOperationException("Something bad happened"));

        // Act
        using var cts = new CancellationTokenSource(500);
        await _serviceUnderTest.StartAsync(cts.Token);
        await _serviceUnderTest.StopAsync(CancellationToken.None);

        // Assert
        _processorFactoryMock.Verify(
            f => f.GetProcessor(It.IsAny<FileDestination>()),
            Times.Once
        );

        Assert.False(Directory.Exists(tempDir), "The temporary directory must be deleted");
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueueReturnsNull_JustContinuesLoop()
    {
        // Arrange
        _queueMock
            .SetupSequence(q => q.DequeueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((FileProcessingTask?)null)
            .ThrowsAsync(new OperationCanceledException());

        // Act
        using var cts = new CancellationTokenSource(500);
        await _serviceUnderTest.StartAsync(cts.Token);
        await _serviceUnderTest.StopAsync(CancellationToken.None);
        
        await _serviceUnderTest.StopAsync(CancellationToken.None);

        // Assert
        _processorFactoryMock.Verify(
            f => f.GetProcessor(It.IsAny<FileDestination>()),
            Times.Never
        );
    }
}
