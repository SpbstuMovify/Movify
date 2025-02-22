using ChunkerService.Dtos.Chunker;
using ChunkerService.Dtos.Hls;
using ChunkerService.Grpc;
using ChunkerService.Hls;
using ChunkerService.Repositories;
using ChunkerService.Services;
using ChunkerService.Utils.Configuration;

using Microsoft.Extensions.Options;

namespace ChunkerService.FileProcessing.FileProcessors;

public class HlsFileProcessor(
    ILogger<HlsFileProcessor> logger,
    IHlsCreator hlsCreator,
    IFileService fileService,
    IOptions<HlsOptions> hlsOptions
) : IFileProcessor
{
    public FileProcessorType Type => FileProcessorType.Hls;

    public async Task ProcessAsync(
        FileProcessorRequest request,
        IServiceScope scope,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation($"Processing file: {request.Key} in {request.BucketName}");

        var chunkerRepository = scope.ServiceProvider.GetRequiredService<IChunkerRepository>();
        var mediaGrpcClient = scope.ServiceProvider.GetRequiredService<IMediaGrpcClient>();

        var prefix = request.Key[..request.Key.LastIndexOf('/')];

        var tokenGuid = Guid.NewGuid();
        logger.LogInformation($"Cancellation token created for HlsCreator with guid[{tokenGuid}]");

        var token = hlsCreator.CreateToken(tokenGuid);

        try
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, token);
            var linkedToken = linkedCts.Token;

            var fileToProcess = await chunkerRepository.DownloadFileAsync(request.BucketName, request.Key);

            var fileToProcessPath = Path.Combine(request.Path, fileToProcess.FileName);

            await using (var fileStream = fileService.CreateFile(fileToProcessPath))
            {
                await fileToProcess.Content.CopyToAsync(fileStream, cancellationToken);
            }

            var hlsStreamPath = Path.Combine(request.Path, "hls");

            await hlsCreator.CreateHlsMasterPlaylistAsync(
                new HlsParamsDto(
                    hlsOptions.Value.FfmpegPath,
                    fileToProcessPath,
                    hlsStreamPath,
                    hlsOptions.Value.Variants.Select(v => new HlsVariantDto(v.Name, v.Width, v.Height, v.VideoBitrate)).ToList(),
                    hlsOptions.Value.SegmentDuration,
                    hlsOptions.Value.AudioBitrate,
                    hlsOptions.Value.AdditionalFfmpegArgs ?? ""
                ),
                linkedToken
            );

            foreach (var filePath in fileService.GetFiles(hlsStreamPath))
            {
                await using var fileStream = fileService.OpenReadFile(filePath);
                var newFileName = Path.GetFileName(filePath);
                var newKey = $"{prefix}/hls/{newFileName}";

                var contentType = "application/octet-stream";
                if (newFileName.EndsWith(".m3u8", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "application/vnd.apple.mpegurl";
                }
                else if (newFileName.EndsWith(".ts", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "video/mp2t";
                }

                await chunkerRepository.UploadFileAsync(
                    new FileData(
                        fileStream,
                        contentType,
                        Path.GetFileName(filePath)
                    ),
                    request.BucketName,
                    newKey
                );
            }

            var masterKey = $"{prefix}/hls/master.m3u8";

            await mediaGrpcClient.ProcessVideoCallback(
                new ProcessVideoCallbackDto(
                    request.BucketName,
                    masterKey,
                    request.BaseUrl,
                    null
                )
            );
        }
        catch (Exception ex)
        {
            await mediaGrpcClient.ProcessVideoCallback(
                new ProcessVideoCallbackDto(
                    request.BucketName,
                    request.Key,
                    null,
                    $"{ex.Message}"
                )
            );
            throw;
        }
        finally
        {
            hlsCreator.CancelToken(tokenGuid);
        }
    }
}
