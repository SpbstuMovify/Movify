using MediaService.Dtos.Chunker;
using MediaService.Dtos.Content;

namespace MediaService.Grpc;

public class ChunckerGrpcClient(Movify.ChunkerService.ChunkerServiceClient client) : IChunckerGrpcClient
{
    public async Task ProcessVideo(ProcessVideoDto processVideoDto)
    {
        await client.ProcessVideoAsync(new Movify.ProcessVideoRequest
        {
            BucketName = processVideoDto.BucketName,
            Key = processVideoDto.Key,
            BaseUrl = processVideoDto.BaseUrl
        });
    }
}