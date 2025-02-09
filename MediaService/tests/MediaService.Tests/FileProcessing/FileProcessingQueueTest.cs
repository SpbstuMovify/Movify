using JetBrains.Annotations;

using MediaService.Dtos.FileInfo;
using MediaService.FileProcessing;

namespace MediaService.Tests.FileProcessing;

[TestSubject(typeof(FileProcessingQueue))]
public class FileProcessingQueueTest
{
    [Fact]
    public async Task DequeueAsync_WhenQueueIsEmpty_ThrowsOperationCanceledException()
    {
        // Arrange
        var queue = new FileProcessingQueue();
        using var cts = new CancellationTokenSource(millisecondsDelay: 100);

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => queue.DequeueAsync(cts.Token));
    }

    [Fact]
    public async Task Enqueue_WhenCalled_DequeueReturnsSameTask()
    {
        // Arrange
        var queue = new FileProcessingQueue();
        var fileTask = new FileProcessingTask(
            new UploadedFileInfoDto("content/path", "image/png", "test.png"),
            "test-bucket",
            "key/path",
            false,
            FileDestination.Internal,
            "http://example.com"
        );

        // Act
        queue.Enqueue(fileTask);
        var result = await queue.DequeueAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileTask, result);
    }

    [Fact]
    public async Task Enqueue_WithMultipleTasks_DequeueReturnsDequeuesInOrder()
    {
        // Arrange
        var queue = new FileProcessingQueue();
        var task1 = new FileProcessingTask(
            new UploadedFileInfoDto("path/1", "video/mp4", "file1.mp4"),
            "bucket1",
            "key1",
            true,
            FileDestination.EpisodeVideoUrl,
            "http://example1.com"
        );
        var task2 = new FileProcessingTask(
            new UploadedFileInfoDto("path/2", "image/png", "file2.png"),
            "bucket2",
            "key2",
            false,
            FileDestination.ContentImageUrl,
            "http://example2.com"
        );

        // Act
        queue.Enqueue(task1);
        queue.Enqueue(task2);

        // Assert
        var dequeued1 = await queue.DequeueAsync(CancellationToken.None);
        var dequeued2 = await queue.DequeueAsync(CancellationToken.None);

        Assert.Equal(task1, dequeued1);
        Assert.Equal(task2, dequeued2);
    }
}
