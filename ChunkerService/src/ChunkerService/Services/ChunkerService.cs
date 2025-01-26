using ChunkerService.Dtos.Chunker;
using ChunkerService.Utils.FileProcessing;

namespace ChunkerService.Services;

public class ChunkerService(IFileProcessingQueue fileProcessingQueue) : IChunkerService
{
    public void ProcessVideo(ProcessVideoDto processVideoDto)
    {
        var bucketName = processVideoDto.BucketName;
        var key = processVideoDto.Key;
        var baseUrl = processVideoDto.BaseUrl;

        fileProcessingQueue.Enqueue(new FileProcessingTask(bucketName, key, baseUrl));
    }
}
