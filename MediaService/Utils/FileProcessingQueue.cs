using System.Threading.Channels;

namespace MediaService.Utils;

public class FileProcessingQueue : IFileProcessingQueue
{
    private readonly Channel<FileProcessingTask> _queue = Channel.CreateUnbounded<FileProcessingTask>();

    public void Enqueue(FileProcessingTask task)
    {
        _queue.Writer.TryWrite(task);
    }

    public async Task<FileProcessingTask?> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
