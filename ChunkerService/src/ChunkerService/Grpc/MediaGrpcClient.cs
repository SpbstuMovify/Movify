using ChunkerService.Dtos.Chunker;

namespace ChunkerService.Grpc;

public class MediaGrpcClient(Movify.MediaService.MediaServiceClient mediaServiceClient) : IMediaGrpcClient
{
    public async Task ProcessVideoCallback(ProcessVideoCallbackDto processVideoCallbackDto)
    {
        if (string.IsNullOrWhiteSpace(processVideoCallbackDto.BucketName)) throw new ArgumentException("BucketName cannot be null or empty", nameof(processVideoCallbackDto));
        if (string.IsNullOrWhiteSpace(processVideoCallbackDto.Key)) throw new ArgumentException("Key cannot be null or empty", nameof(processVideoCallbackDto));

        await mediaServiceClient.ProcessVideoCallbackAsync(
            new Movify.ProcessVideoCallbackRequest
            {
                BucketName = processVideoCallbackDto.BucketName,
                Key = processVideoCallbackDto.Key,
                BaseUrl = processVideoCallbackDto.BaseUrl ?? "",
                Error = processVideoCallbackDto.Error ?? ""
            }
        );
    }
}
