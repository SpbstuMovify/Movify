using ChunkerService.Dtos.Chunker;
using ChunkerService.FileProcessing;

using JetBrains.Annotations;

using Moq;

namespace ChunkerService.Tests.Services;

[TestSubject(typeof(ChunkerService.Services.ChunkerService))]
public class ChunkerServiceTest
{
    [Fact]
    public void ProcessVideo_ShouldEnqueueFileProcessingTask_WithCorrectValues()
    {
        // Arrange
        var mockQueue = new Mock<IFileProcessingQueue>();
        var chunkerService = new ChunkerService.Services.ChunkerService(mockQueue.Object);

        var processVideoDto = new ProcessVideoDto(
            "test-bucket",
            "test-video.mp4",
            "https://example.com"
        );

        // Act
        chunkerService.ProcessVideo(processVideoDto);

        // Assert
        mockQueue.Verify(
            q => q.Enqueue(
                It.Is<FileProcessingTask>(
                    task =>
                        task.BucketName == processVideoDto.BucketName &&
                        task.Key == processVideoDto.Key &&
                        task.BaseUrl == processVideoDto.BaseUrl
                )
            ),
            Times.Once
        );
    }
}
