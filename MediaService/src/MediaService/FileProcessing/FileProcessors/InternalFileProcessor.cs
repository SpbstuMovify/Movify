namespace MediaService.FileProcessing.FileProcessors;

public class InternalFileProcessor(ILogger<InternalFileProcessor> logger) : FileProcessorBase
{
    public override FileDestination Destination => FileDestination.Internal;

    public override async Task ProcessAsync(
        FileProcessorRequest request,
        IServiceScope scope
    )
    {
        var uploadedData = await UploadFileAsync(request, scope);
        logger.LogInformation("Internal file uploaded: {Url}", uploadedData.PresignedUrl);
    }
}
