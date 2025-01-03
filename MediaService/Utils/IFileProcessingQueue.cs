namespace MediaService.Utils;

public interface IFileProcessingQueue
{
    void Enqueue(FileProcessingTask task);
    Task<FileProcessingTask?> DequeueAsync(CancellationToken cancellationToken);
}
