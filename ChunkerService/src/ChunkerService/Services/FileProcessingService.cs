using ChunkerService.FileProcessing;
using ChunkerService.FileProcessing.FileProcessors;

namespace ChunkerService.Services;

public class FileProcessingService(
    ILogger<FileProcessingService> logger,
    IFileProcessingQueue fileProcessingQueue,
    IServiceScopeFactory serviceScopeFactory,
    IFileService fileService,
    IFileProcessorFactory fileProcessorFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("File Processing Service started.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var task = await fileProcessingQueue.DequeueAsync(cancellationToken);

                if (task == null)
                {
                    continue;
                }

                await ProcessTaskAsync(task, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing file");
            }
        }
    }

    private async Task ProcessTaskAsync(
        FileProcessingTask task,
        CancellationToken cancellationToken
    )
    {
        var path = Path.Combine(".tmp", Guid.NewGuid().ToString());
        if (!fileService.DirectoryExists(path))
        {
            fileService.CreateDirectory(path);
        }

        try
        {
            using var scope = serviceScopeFactory.CreateScope();

            var processor = fileProcessorFactory.GetProcessor(FileProcessorType.Hls);
            await processor.ProcessAsync(
                new FileProcessorRequest(
                    path,
                    task.BucketName,
                    task.Key,
                    task.BaseUrl
                ),
                scope,
                cancellationToken
            );
        }
        finally
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory) && fileService.DirectoryExists(directory))
            {
                fileService.DeleteDirectory(directory, true);
                logger.LogDebug("Cleaned up directory: {Directory}", directory);
            }
        }
    }
}
