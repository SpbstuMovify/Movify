using MediaService.FileProcessing;
using MediaService.FileProcessing.FileProcessors;
using MediaService.Repositories;

namespace MediaService.Services;

public class FileProcessingService(
    IFileProcessingQueue fileProcessingQueue,
    IServiceScopeFactory serviceScopeFactory,
    IFileProcessorFactory fileProcessorFactory,
    ILogger<FileProcessingServiceV1> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("File Processing Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var task = await fileProcessingQueue.DequeueAsync(stoppingToken);

                if (task == null)
                {
                    continue;
                }

                await ProcessTaskAsync(task);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing file");
            }
        }
    }

    private async Task ProcessTaskAsync(FileProcessingTask task)
    {
        try
        {
            await using var stream = new FileStream(task.File.ContentPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var scope = serviceScopeFactory.CreateScope();

            var processor = fileProcessorFactory.GetProcessor(task.Destination);
            await processor.ProcessAsync(
                new FileProcessorRequest(
                    new FileData(
                        stream,
                        task.File.ContentType,
                        task.File.FileName
                    ),
                    task.BucketName,
                    task.Key,
                    task.BaseUrl,
                    task.IsVideoProcNecessary
                ),
                scope
            );
        }
        finally
        {
            var directory = Path.GetDirectoryName(task.File.ContentPath);
            if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
                logger.LogDebug("Cleaned up directory: {Directory}", directory);
            }
        }
    }
}
