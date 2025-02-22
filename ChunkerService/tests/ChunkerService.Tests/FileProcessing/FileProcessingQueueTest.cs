using ChunkerService.FileProcessing;

using JetBrains.Annotations;

namespace ChunkerService.Tests.FileProcessing;

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
        var fileTask = new FileProcessingTask("bucket1", "key1", "baseUrl1");

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
        var task1 = new FileProcessingTask("bucket1", "key1", "baseUrl1");
        var task2 = new FileProcessingTask("bucket2", "key2", "baseUrl2");

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
