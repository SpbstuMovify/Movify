using ChunkerService.FileProcessing;
using ChunkerService.FileProcessing.FileProcessors;
using ChunkerService.Services;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace ChunkerService.Tests.Services;

[TestSubject(typeof(FileProcessingService))]
public class FileProcessingServiceTest
{
    private readonly Mock<ILogger<FileProcessingService>> _loggerMock;
    private readonly Mock<IFileProcessingQueue> _queueMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IFileProcessor> _processorMock;
    private readonly FileProcessingService _serviceUnderTest;

    public FileProcessingServiceTest()
    {
        _loggerMock = new Mock<ILogger<FileProcessingService>>();
        _queueMock = new Mock<IFileProcessingQueue>();
        _fileServiceMock = new Mock<IFileService>();
        _processorMock = new Mock<IFileProcessor>();
        
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        var scopeMock = new Mock<IServiceScope>();

        scopeFactoryMock.Setup(s => s.CreateScope()).Returns(scopeMock.Object);

        _processorMock.SetupGet(p => p.Type).Returns(FileProcessorType.Hls);
        var processorFactory = new FileProcessorFactory(new List<IFileProcessor> { _processorMock.Object });

        _serviceUnderTest = new FileProcessingService(
            _loggerMock.Object,
            _queueMock.Object,
            scopeFactoryMock.Object,
            _fileServiceMock.Object,
            processorFactory
        );
    }

    [Fact]
    public async Task ExecuteAsync_HappyPath_ProcessesFileAndCleansDirectory()
    {
        // Arrange
        var fileTask = new FileProcessingTask("test-bucket", "some/key", "http://example.com");

        _queueMock.SetupSequence(q => q.DequeueAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(fileTask)
                  .ThrowsAsync(new OperationCanceledException());

        _fileServiceMock.Setup(fs => fs.DirectoryExists(It.Is<string>(s => s.StartsWith(".tmp" + Path.DirectorySeparatorChar))))
                        .Returns(false);

        _fileServiceMock.Setup(fs => fs.DirectoryExists(".tmp"))
                        .Returns(true);

        _processorMock.Setup(p => p.ProcessAsync(It.IsAny<FileProcessorRequest>(), It.IsAny<IServiceScope>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask)
                      .Verifiable();

        _fileServiceMock.Setup(fs => fs.CreateDirectory(It.Is<string>(s => s.StartsWith(".tmp"))));
        _fileServiceMock.Setup(fs => fs.DeleteDirectory(".tmp", true));

        // Act
        using var cts = new CancellationTokenSource(500);
        await _serviceUnderTest.StartAsync(cts.Token);
        await Task.Delay(100);
        await _serviceUnderTest.StopAsync(CancellationToken.None);

        // Assert
        _processorMock.Verify(
            p => p.ProcessAsync(
                It.Is<FileProcessorRequest>(
                    req =>
                        req.BucketName == fileTask.BucketName &&
                        req.Key == fileTask.Key &&
                        req.BaseUrl == fileTask.BaseUrl &&
                        req.Path.StartsWith(".tmp")
                ),
                It.IsAny<IServiceScope>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        _fileServiceMock.Verify(fs => fs.CreateDirectory(It.Is<string>(s => s.StartsWith(".tmp"))), Times.Once);
        _fileServiceMock.Verify(fs => fs.DeleteDirectory(".tmp", true), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProcessTaskThrows_LogsErrorAndCleansDirectory()
    {
        // Arrange
        var fileTask = new FileProcessingTask("bucket-error", "error/key", "http://example.com");

        _queueMock.SetupSequence(q => q.DequeueAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(fileTask)
                  .ThrowsAsync(new OperationCanceledException());

        _fileServiceMock.Setup(fs => fs.DirectoryExists(It.Is<string>(s => s.StartsWith(".tmp" + Path.DirectorySeparatorChar))))
                        .Returns(false);
        _fileServiceMock.Setup(fs => fs.DirectoryExists(".tmp"))
                        .Returns(true);

        var testException = new Exception("Processing failed");
        _processorMock.Setup(p => p.ProcessAsync(It.IsAny<FileProcessorRequest>(), It.IsAny<IServiceScope>(), It.IsAny<CancellationToken>()))
                      .ThrowsAsync(testException);

        _fileServiceMock.Setup(fs => fs.CreateDirectory(It.IsAny<string>()));
        _fileServiceMock.Setup(fs => fs.DeleteDirectory(It.IsAny<string>(), true));

        // Act
        using var cts = new CancellationTokenSource(500);
        await _serviceUnderTest.StartAsync(cts.Token);
        await Task.Delay(100);
        await _serviceUnderTest.StopAsync(CancellationToken.None);

        // Assert
        _processorMock.Verify(
            p => p.ProcessAsync(
                It.Is<FileProcessorRequest>(
                    req =>
                        req.BucketName == fileTask.BucketName &&
                        req.Key == fileTask.Key &&
                        req.BaseUrl == fileTask.BaseUrl &&
                        req.Path.StartsWith(".tmp")
                ),
                It.IsAny<IServiceScope>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        _fileServiceMock.Verify(fs => fs.DeleteDirectory(".tmp", true), Times.Once);
        _loggerMock.Verify(
            l => l.Log(
                It.Is<LogLevel>(lvl => lvl == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>(
                    (
                        v,
                        t
                    ) => v.ToString()!.Contains("Error processing file")
                ),
                testException,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueueReturnsNull_ContinuesLoop()
    {
        // Arrange
        _queueMock.SetupSequence(q => q.DequeueAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync((FileProcessingTask?)null)
                  .ThrowsAsync(new OperationCanceledException());

        // Act
        using var cts = new CancellationTokenSource(500);
        await _serviceUnderTest.StartAsync(cts.Token);
        await Task.Delay(100);
        await _serviceUnderTest.StopAsync(CancellationToken.None);

        // Assert
        _processorMock.Verify(p => p.ProcessAsync(It.IsAny<FileProcessorRequest>(), It.IsAny<IServiceScope>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
