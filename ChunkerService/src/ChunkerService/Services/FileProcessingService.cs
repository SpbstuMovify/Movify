using ChunkerService.Dtos.Chunker;
using ChunkerService.Grpc;
using ChunkerService.Hls;
using ChunkerService.Repositories;
using ChunkerService.Utils.Configuration;
using ChunkerService.Utils.FileProcessing;
using Microsoft.Extensions.Options;

namespace ChunkerService.Services;

public class FileProcessingService(
    IFileProcessingQueue fileProcessingQueue,
    IHlsCreator hlsCreator,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<FileProcessingService> logger,
    IOptions<HlsOptions> options
) : BackgroundService
{
    private readonly HlsOptions _options = options.Value;

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
                        await ProcessFileAsync(scope, task, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing file");
            }
        }
    }

    private async Task ProcessFileAsync(IServiceScope scope, FileProcessingTask task, CancellationToken stoppingToken)
    {
        logger.LogInformation($"Processing file: {task.Key} in {task.BucketName}");

        var chunkerRepository = scope.ServiceProvider.GetRequiredService<IChunkerRepository>();
        var mediaGrpcClient = scope.ServiceProvider.GetRequiredService<IMediaGrpcClient>();

        var bucketName = task.BucketName;
        var key = task.Key;
        var baseUrl = task.BaseUrl;
        (var prefix, var fileName) = SplitPath(key);

        var tokenGuid = Guid.NewGuid();
        logger.LogInformation($"Cancellation token created for HlsCreator with guid[{tokenGuid}]");

        var token = hlsCreator.CreateToken(tokenGuid);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, token);
        var linkedToken = linkedCts.Token;

        var path = Path.Combine(".tmp", tokenGuid.ToString());
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        try
        {
            var fileToProccess = await chunkerRepository.DownloadFileAsync(bucketName, key);

            string fileToProcessPath = Path.Combine(path, fileToProccess.FileName);

            using (FileStream fileStream = new FileStream(fileToProcessPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await fileToProccess.Content.CopyToAsync(fileStream);
            }

            string hlsStreamPath = Path.Combine(path, "hls");

            await CreateHlsStream(hlsStreamPath, fileToProcessPath, linkedToken);
            hlsCreator.CancelToken(tokenGuid);

            foreach (string filePath in Directory.GetFiles(hlsStreamPath))
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    var newFileName = Path.GetFileName(filePath);
                    var newKey = $"{prefix}/hls/{newFileName}";

                    string contentType = "application/octet-stream";
                    if (newFileName.EndsWith(".m3u8", StringComparison.OrdinalIgnoreCase))
                    {
                        contentType = "application/vnd.apple.mpegurl";
                    }
                    else if (newFileName.EndsWith(".ts", StringComparison.OrdinalIgnoreCase))
                    {
                        contentType = "video/mp2t";
                    }

                    await chunkerRepository.UploadFileAsync(new UploadedFile
                    {
                        Content = fileStream,
                        ContentType = contentType,
                        FileName = Path.GetFileName(filePath)
                    }, bucketName, newKey);
                }
            }

            Directory.Delete(path, true);

            var masterKey = $"{prefix}/hls/master.m3u8";

            await mediaGrpcClient.ProcessVideoCallback(new ProcessVideoDtoCallbackDto
            {
                BucketName = bucketName,
                Key = masterKey,
                BaseUrl = baseUrl
            });
        }
        catch (Exception ex)
        {
            hlsCreator.CancelToken(tokenGuid);
            Directory.Delete(path, true);

            await mediaGrpcClient.ProcessVideoCallback(new ProcessVideoDtoCallbackDto
            {
                BucketName = bucketName,
                Key = key,
                Error = $"{ex.Message}"
            });
            throw;
        }
    }

    private async Task CreateHlsStream(string hlsStreamPath, string filePath, CancellationToken stoppingToken)
    {
        var variants = _options.Variants.ToList();
        await hlsCreator.CreateHlsMasterPlaylistAsync("ffmpeg", filePath, hlsStreamPath, variants, cancellationToken: stoppingToken);
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
}
