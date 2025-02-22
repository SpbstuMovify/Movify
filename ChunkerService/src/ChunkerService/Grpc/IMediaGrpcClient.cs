using ChunkerService.Dtos.Chunker;

namespace ChunkerService.Grpc;

public interface IMediaGrpcClient
{
    Task ProcessVideoCallback(ProcessVideoCallbackDto processVideoCallbackDto);
}
