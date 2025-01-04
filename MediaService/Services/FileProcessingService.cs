using MediaService.Dtos.Content;
using MediaService.Grpc;
using MediaService.Repositories;
using MediaService.Utils.FileProcessing;

namespace MediaService.Services;

public class FileProcessingService(
    IFileProcessingQueue fileProcessingQueue,
    IServiceScopeFactory serviceScopeFactory,
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
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        await ProcessFileAsync(scope, task);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing file");
            }
        }
    }

    private async Task ProcessFileAsync(IServiceScope scope, FileProcessingTask task)
    {
        logger.LogInformation($"Processing file: {task.Key} in {task.BucketName}");

        try
        {
            switch (task.Destination)
            {
                case FileDestination.Internal:
                    await ProcessInternalFileAsync(scope, task);
                    return;
                case FileDestination.ContentImageUrl:
                    await ProcessContentImageAsync(scope, task);
                    return;
                case FileDestination.EpisodeVideoUrl:
                    await ProcessEpisodeVideoAsync(scope, task);
                    return;
                default:
                    return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to process file: {task.Key}");
        }
    }

    private async Task ProcessInternalFileAsync(IServiceScope scope, FileProcessingTask task)
    {
        var bucketRepository = scope.ServiceProvider.GetRequiredService<IBucketRepository>();

        var file = task.File;
        var bucketName = task.BucketName;
        var key = task.Key;

        var data = await bucketRepository.UploadFileAsync(file, bucketName, key);
        logger.LogInformation($"File uploaded: {data.PresignedUrl}");
    }

    private async Task ProcessContentImageAsync(IServiceScope scope, FileProcessingTask task)
    {
        var bucketRepository = scope.ServiceProvider.GetRequiredService<IBucketRepository>();
        var contentGrpcClient = scope.ServiceProvider.GetRequiredService<IContentGrpcClient>();

        var file = task.File;
        var bucketName = task.BucketName;
        var key = task.Key;
        var contentId = ParseSegment(task.Key, 0);
        var url = $"{task.BaseUrl}/{task.Key}";

        var data = await bucketRepository.UploadFileAsync(file, bucketName, key);
        logger.LogInformation($"File uploaded: {data.PresignedUrl}");

        await contentGrpcClient.SetContentImageUrl(new ContentImageUrlDto
        {
            ContentId = contentId,
            Url = url
        });
    }

    private async Task ProcessEpisodeVideoAsync(IServiceScope scope, FileProcessingTask task)
    {
        var bucketRepository = scope.ServiceProvider.GetRequiredService<IBucketRepository>();
        var contentGrpcClient = scope.ServiceProvider.GetRequiredService<IContentGrpcClient>();

        var file = task.File;
        var bucketName = task.BucketName;
        var key = task.Key;
        var episodeId = ParseSegment(task.Key, 1);
        var url = $"{task.BaseUrl}/{task.Key}";

        await contentGrpcClient.SetEpisodeVideoUrlDto(new EpisodeVideoUrlDto
        {
            EpisodeId = episodeId,
            Url = "",
            Status = FileStatus.InProgress
        });

        try
        {
            var data = await bucketRepository.UploadFileAsync(file, bucketName, key);
            logger.LogInformation($"File uploaded: {data.PresignedUrl}");

            await contentGrpcClient.SetEpisodeVideoUrlDto(new EpisodeVideoUrlDto
            {
                EpisodeId = episodeId,
                Url = url,
                Status = FileStatus.Uploaded
            });
        }
        catch (Exception)
        {
            await contentGrpcClient.SetEpisodeVideoUrlDto(new EpisodeVideoUrlDto
            {
                EpisodeId = episodeId,
                Url = "",
                Status = FileStatus.Error
            });
            throw;
        }
    }

    private static string ParseSegment(string path, int index)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        string[] parts = path.Split('/');
        if (index < 0 || index >= parts.Length)
            return string.Empty;

        return parts[index];
    }
}
