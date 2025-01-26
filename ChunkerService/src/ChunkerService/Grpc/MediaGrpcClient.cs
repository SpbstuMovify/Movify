using ChunkerService.Dtos.Chunker;

namespace ChunkerService.Grpc;

public class MediaGrpcClient(Movify.MediaService.MediaServiceClient mediaServiceClient) : IMediaGrpcClient
{
    public async Task ProcessVideoCallback(ProcessVideoDtoCallbackDto processVideoDto)
    {
        await mediaServiceClient.ProcessVideoCallbackAsync(new Movify.ProcessVideoCallbackRequest
        {
            BucketName = processVideoDto.BucketName,
            Key = processVideoDto.Key,
            BaseUrl = processVideoDto.BaseUrl ?? "",
            Error = processVideoDto.Error ?? ""
        });
    }
}
