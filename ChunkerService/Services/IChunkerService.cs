using ChunkerService.Dtos.Chunker;

namespace ChunkerService.Services;

public interface IChunkerService
{
    void ProcessVideo(ProcessVideoDto processVideoDto);
}