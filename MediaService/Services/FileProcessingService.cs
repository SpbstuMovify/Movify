using MediaService.Repositories;
using MediaService.Utils.FileProcessing;

namespace MediaService.Services;

public class FileProcessingService(
    IFileProcessingQueue fileProcessingQueue,
    IBucketRepository bucketRepository,
    ILogger<FileProcessingService> logger
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
                if (task != null)
                {
                    await ProcessFileAsync(task);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing file");
            }
        }
    }

    private async Task ProcessFileAsync(FileProcessingTask task)
    {
        logger.LogInformation($"Processing file: {task.Key} in {task.BucketName}");

        try
        {
            var data = await bucketRepository.UploadFileAsync(task.File, task.BucketName, task.Key);
            logger.LogInformation($"File uploaded: {data.PresignedUrl}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to process file: {task.Key}");
        }
    }
}
