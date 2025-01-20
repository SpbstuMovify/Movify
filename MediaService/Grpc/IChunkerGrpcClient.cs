using MediaService.Dtos.Chunker;
using MediaService.Dtos.Content;

namespace MediaService.Grpc;

public interface IChunckerGrpcClient
{
    Task ProcessVideo(ProcessVideoDto processVideoDto);
}