using MediaService.Dtos.Chunker;

namespace MediaService.Grpc.Clients;

public class ChunkerGrpcClient(Movify.ChunkerService.ChunkerServiceClient client) : IChunckerGrpcClient
{
    public async Task ProcessVideo(ProcessVideoDto processVideoDto)
    {
        if (string.IsNullOrWhiteSpace(processVideoDto.BucketName)) throw new ArgumentException("BucketName cannot be null or empty", nameof(processVideoDto));
        if (string.IsNullOrWhiteSpace(processVideoDto.Key)) throw new ArgumentException("Key cannot be null or empty", nameof(processVideoDto));
        if (string.IsNullOrWhiteSpace(processVideoDto.BaseUrl)) throw new ArgumentException("BaseUrl cannot be null or empty", nameof(processVideoDto));
        
        await client.ProcessVideoAsync(
            new Movify.ProcessVideoRequest
            {
                BucketName = processVideoDto.BucketName,
                Key = processVideoDto.Key,
                BaseUrl = processVideoDto.BaseUrl
            }
        );
    }
}
