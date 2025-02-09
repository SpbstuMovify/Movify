using JetBrains.Annotations;

using MediaService.FileProcessing;
using MediaService.FileProcessing.FileProcessors;

using Moq;

namespace MediaService.Tests.FileProcessing.FileProcessors;

[TestSubject(typeof(FileProcessorFactory))]
public class FileProcessorFactoryTest
{
    private readonly Mock<IFileProcessor> _internalProcessorMock;
    private readonly Mock<IFileProcessor> _contentImageProcessorMock;
    private readonly Mock<IFileProcessor> _episodeVideoProcessorMock;
    private readonly IFileProcessorFactory _fileProcessorFactory;

    public FileProcessorFactoryTest()
    {
        _internalProcessorMock = new Mock<IFileProcessor>();
        _internalProcessorMock
            .SetupGet(p => p.Destination)
            .Returns(FileDestination.Internal);

        _contentImageProcessorMock = new Mock<IFileProcessor>();
        _contentImageProcessorMock
            .SetupGet(p => p.Destination)
            .Returns(FileDestination.ContentImageUrl);

        _episodeVideoProcessorMock = new Mock<IFileProcessor>();
        _episodeVideoProcessorMock
            .SetupGet(p => p.Destination)
            .Returns(FileDestination.EpisodeVideoUrl);

        var processors = new List<IFileProcessor>
        {
            _internalProcessorMock.Object,
            _contentImageProcessorMock.Object,
            _episodeVideoProcessorMock.Object
        };

        _fileProcessorFactory = new FileProcessorFactory(processors);
    }

    [Fact]
    public void GetProcessor_WithInternalDestination_ReturnsInternalProcessor()
    {
        // Act
        var processor = _fileProcessorFactory.GetProcessor(FileDestination.Internal);

        // Assert
        Assert.Same(_internalProcessorMock.Object, processor);
    }

    [Fact]
    public void GetProcessor_WithContentImageUrlDestination_ReturnsContentImageProcessor()
    {
        // Act
        var processor = _fileProcessorFactory.GetProcessor(FileDestination.ContentImageUrl);

        // Assert
        Assert.Same(_contentImageProcessorMock.Object, processor);
    }

    [Fact]
    public void GetProcessor_WithEpisodeVideoUrlDestination_ReturnsEpisodeVideoProcessor()
    {
        // Act
        var processor = _fileProcessorFactory.GetProcessor(FileDestination.EpisodeVideoUrl);

        // Assert
        Assert.Same(_episodeVideoProcessorMock.Object, processor);
    }

    [Fact]
    public void GetProcessor_WithUnknownDestination_ThrowsArgumentException()
    {
        // Arrange
        var unknownDestination = (FileDestination)999;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _fileProcessorFactory.GetProcessor(unknownDestination));
        Assert.Contains("No processor to assign", ex.Message);
    }
}
