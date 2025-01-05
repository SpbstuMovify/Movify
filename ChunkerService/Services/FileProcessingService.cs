using ChunkerService.Dtos.Chunker;
using ChunkerService.Grpc;
using ChunkerService.Repositories;
using ChunkerService.Utils.FileProcessing;

namespace ChunkerService.Services;

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

        var chunkerRepository = scope.ServiceProvider.GetRequiredService<IChunkerRepository>();
        var mediaGrpcClient = scope.ServiceProvider.GetRequiredService<IMediaGrpcClient>();

        var bucketName = task.BucketName;
        var key = task.Key;
        var baseUrl = task.BaseUrl;
        (var prefix, var fileName) = SplitPath(key);
        var fileExtension = GetFileExtension(fileName);

        try
        {
            var fileToProccess = await chunkerRepository.DownloadFileAsync(bucketName, key);

            var newFileName = $"NewFile.{fileExtension}";
            var newKey = $"{prefix}/{newFileName}";

            var newFileInfo = await chunkerRepository.UploadFileAsync(new UploadedFile
            {
                Content = fileToProccess.Content,
                ContentType = fileToProccess.ContentType,
                FileName = newFileName
            }, bucketName, newKey);

            await mediaGrpcClient.ProcessVideoCallback(new ProcessVideoDtoCallbackDto
            {
                BucketName = bucketName,
                Key = newKey,
                BaseUrl = baseUrl
            });
        }
        catch (Exception ex)
        {
            await mediaGrpcClient.ProcessVideoCallback(new ProcessVideoDtoCallbackDto
            {
                BucketName = bucketName,
                Key = key,
                Error = $"{ex.Message}"
            });
            throw;
        }
    }

    private static (string, string) SplitPath(string input)
    {
        if (string.IsNullOrEmpty(input) || !input.Contains("/"))
        {
            return (string.Empty, input);
        }

        int lastIndex = input.LastIndexOf('/');
        string part1 = input.Substring(0, lastIndex);
        string part2 = input.Substring(lastIndex + 1);

        return (part1, part2);
    }

    private static string GetFileExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || !fileName.Contains("."))
        {
            return string.Empty;
        }

        int lastIndex = fileName.LastIndexOf('.');
        return fileName.Substring(lastIndex + 1);
    }
}
