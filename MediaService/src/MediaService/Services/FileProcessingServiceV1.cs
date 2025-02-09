using MediaService.Dtos.Chunker;
using MediaService.Dtos.Content;
using MediaService.FileProcessing;
using MediaService.Grpc;
using MediaService.Grpc.Clients;
using MediaService.Repositories;

namespace MediaService.Services;

public class FileProcessingServiceV1(
    IFileProcessingQueue fileProcessingQueue,
    IServiceScopeFactory serviceScopeFactory,
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

    private async Task ProcessFileAsync(
        IServiceScope scope,
        FileProcessingTask task
    )
    {
        logger.LogInformation($"Processing file: {task.Key} in {task.BucketName}");

        try
        {
            switch (task.Destination)
            {
                case FileDestination.Internal: await ProcessInternalFileAsync(scope, task); break;
                case FileDestination.ContentImageUrl: await ProcessContentImageAsync(scope, task); break;
                case FileDestination.EpisodeVideoUrl: await ProcessEpisodeVideoAsync(scope, task); break;
                default: break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to process file: {task.Key}");
        }

        var fileDirectory = Path.GetDirectoryName(task.File.ContentPath);
        if (fileDirectory != null)
        {
            Directory.Delete(fileDirectory, true);
        }
    }

    private async Task ProcessInternalFileAsync(
        IServiceScope scope,
        FileProcessingTask task
    )
    {
        var bucketRepository = scope.ServiceProvider.GetRequiredService<IBucketRepository>();

        var file = task.File;
        var bucketName = task.BucketName;
        var key = task.Key;

        using (FileStream fileStream = new FileStream(file.ContentPath, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            var data = await bucketRepository.UploadFileAsync(
                new FileData(
                    fileStream,
                    file.ContentType,
                    file.FileName
                ),
                bucketName,
                key
            );
            logger.LogInformation($"File uploaded: {data.PresignedUrl}");
        }
    }

    private async Task ProcessContentImageAsync(
        IServiceScope scope,
        FileProcessingTask task
    )
    {
        var bucketRepository = scope.ServiceProvider.GetRequiredService<IBucketRepository>();
        var contentGrpcClient = scope.ServiceProvider.GetRequiredService<IContentGrpcClient>();

        var file = task.File;
        var bucketName = task.BucketName;
        var key = task.Key;
        var contentId = ParseSegment(task.Key, 0);
        var url = $"{task.BaseUrl}/{task.Key}";

        using (FileStream fileStream = new FileStream(file.ContentPath, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            var data = await bucketRepository.UploadFileAsync(
                new FileData(
                    fileStream,
                    file.ContentType,
                    file.FileName
                ),
                bucketName,
                key
            );
            logger.LogInformation($"File uploaded: {data.PresignedUrl}");
        }

        await contentGrpcClient.SetContentImageUrl(new ContentImageUrlDto(contentId, url));
    }

    private async Task ProcessEpisodeVideoAsync(
        IServiceScope scope,
        FileProcessingTask task
    )
    {
        var bucketRepository = scope.ServiceProvider.GetRequiredService<IBucketRepository>();
        var contentGrpcClient = scope.ServiceProvider.GetRequiredService<IContentGrpcClient>();

        var file = task.File;
        var bucketName = task.BucketName;
        var key = task.Key;
        var episodeId = ParseSegment(task.Key, 1);
        var url = $"{task.BaseUrl}/{task.Key}";
        var IsVideoProcNecessary = task.IsVideoProcNecessary;

        await contentGrpcClient.SetEpisodeVideoUrl(new EpisodeVideoUrlDto(episodeId, "", FileStatus.InProgress));

        try
        {
            using (FileStream fileStream = new FileStream(file.ContentPath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var data = await bucketRepository.UploadFileAsync(
                    new FileData(
                        fileStream,
                        file.ContentType,
                        file.FileName
                    ),
                    bucketName,
                    key
                );
                logger.LogInformation($"File uploaded: {data.PresignedUrl}");
            }

            await contentGrpcClient.SetEpisodeVideoUrl(
                new EpisodeVideoUrlDto(episodeId, url, FileStatus.Uploaded)
            );

            if (IsVideoProcNecessary)
            {
                var chunkerGrpcClient = scope.ServiceProvider.GetRequiredService<IChunckerGrpcClient>();

                await chunkerGrpcClient.ProcessVideo(
                    new ProcessVideoDto(bucketName, key, task.BaseUrl)
                );
            }
        }
        catch (Exception)
        {
            await contentGrpcClient.SetEpisodeVideoUrl(
                new EpisodeVideoUrlDto(episodeId, "", FileStatus.Error)
            );
            throw;
        }
    }

    private static string ParseSegment(
        string path,
        int index
    )
    {
        if (string.IsNullOrEmpty(path)) return string.Empty;

        string[] parts = path.Split('/');
        if (index < 0 || index >= parts.Length) return string.Empty;

        return parts[index];
    }
}
