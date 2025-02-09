using MediaService.Dtos.Chunker;

namespace MediaService.Grpc.Clients;

public interface IChunckerGrpcClient
{
    Task ProcessVideo(ProcessVideoDto processVideoDto);
}
