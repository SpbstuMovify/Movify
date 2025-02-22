using ChunkerService.FileProcessing.FileProcessors;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace ChunkerService.Tests.FileProcessing.FileProcessors;

[TestSubject(typeof(FileProcessorFactory))]
public class FileProcessorFactoryTest
{
    [Fact]
    public void GetProcessor_ReturnsProcessor_WhenProcessorExists()
    {
        // Arrange
        var dummyHlsProcessor = new DummyFileProcessor(FileProcessorType.Hls);
        var processors = new List<IFileProcessor> { dummyHlsProcessor };
        var factory = new FileProcessorFactory(processors);

        // Act
        var processor = factory.GetProcessor(FileProcessorType.Hls);

        // Assert
        Assert.Same(dummyHlsProcessor, processor);
    }

    [Fact]
    public void GetProcessor_ThrowsArgumentException_WhenProcessorDoesNotExist()
    {
        // Arrange
        var dummyHlsProcessor = new DummyFileProcessor(FileProcessorType.Hls);
        var processors = new List<IFileProcessor> { dummyHlsProcessor };
        var factory = new FileProcessorFactory(processors);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => factory.GetProcessor(FileProcessorType.Other));
        Assert.Contains("No processor to assign", exception.Message);
    }
}

public class DummyFileProcessor(FileProcessorType type) : IFileProcessor
{
    public FileProcessorType Type { get; } = type;

    public Task ProcessAsync(
        FileProcessorRequest request,
        IServiceScope scope,
        CancellationToken cancellationToken = default
    ) =>
        Task.CompletedTask;
}
