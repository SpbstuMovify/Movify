using MediaService.Dtos.Chunker;
using MediaService.Dtos.Content;
using MediaService.Grpc.Clients;

namespace MediaService.FileProcessing.FileProcessors;

public class EpisodeVideoFileProcessor(ILogger<EpisodeVideoFileProcessor> logger) : FileProcessorBase
{
    public override FileDestination Destination => FileDestination.EpisodeVideoUrl;

    public override async Task ProcessAsync(
        FileProcessorRequest request,
        IServiceScope scope
    )
    {
        var contentGrpcClient = scope.ServiceProvider.GetRequiredService<IContentGrpcClient>();

        var episodeId = request.Key.Split('/', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1) ?? string.Empty;
        var url = $"{request.BaseUrl}/{request.Key}";
        var isVideoProcessingNecessary = request.IsVideoProcNecessary;

        await contentGrpcClient.SetEpisodeVideoUrl(new EpisodeVideoUrlDto(episodeId, "", FileStatus.InProgress));

        try
        {
            var uploadedData = await UploadFileAsync(request, scope);
            logger.LogInformation("Episode video uploaded: {Url}", uploadedData.PresignedUrl);

            await contentGrpcClient.SetEpisodeVideoUrl(new EpisodeVideoUrlDto(episodeId, url, FileStatus.Uploaded));

            if (isVideoProcessingNecessary)
            {
                var chunkerGrpcClient = scope.ServiceProvider.GetRequiredService<IChunckerGrpcClient>();
                await chunkerGrpcClient.ProcessVideo(new ProcessVideoDto(request.BucketName, request.Key, request.BaseUrl));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing episode video for EpisodeId: {EpisodeId}", episodeId);
            await contentGrpcClient.SetEpisodeVideoUrl(new EpisodeVideoUrlDto(episodeId, "", FileStatus.Error));
            throw;
        }
    }
}
