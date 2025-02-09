using MediaService.Dtos.Content;
using MediaService.Grpc.Clients;

namespace MediaService.FileProcessing.FileProcessors;

public class ContentImageFileProcessor(ILogger<ContentImageFileProcessor> logger) : FileProcessorBase
{
    public override FileDestination Destination => FileDestination.ContentImageUrl;

    public override async Task ProcessAsync(
        FileProcessorRequest request,
        IServiceScope scope
    )
    {
        var uploadedData = await UploadFileAsync(request, scope);
        logger.LogInformation("Content image uploaded: {Url}", uploadedData.PresignedUrl);

        var contentGrpcClient = scope.ServiceProvider.GetRequiredService<IContentGrpcClient>();

        var contentId = request.Key.Split('/', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(0) ?? string.Empty;
        var url = $"{request.BaseUrl}/{request.Key}";

        await contentGrpcClient.SetContentImageUrl(new ContentImageUrlDto(contentId, url));
    }
}
