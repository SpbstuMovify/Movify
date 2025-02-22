using System.Threading.Channels;

namespace ChunkerService.FileProcessing;

public class FileProcessingQueue : IFileProcessingQueue
{
    private readonly Channel<FileProcessingTask> _queue = Channel.CreateUnbounded<FileProcessingTask>();

    public void Enqueue(FileProcessingTask task) => _queue.Writer.TryWrite(task);

    public async Task<FileProcessingTask?> DequeueAsync(CancellationToken cancellationToken) => await _queue.Reader.ReadAsync(cancellationToken);
}
