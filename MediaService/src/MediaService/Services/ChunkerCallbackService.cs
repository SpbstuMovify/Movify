using MediaService.Dtos.Chunker;
using MediaService.Dtos.Content;
using MediaService.FileProcessing;
using MediaService.Grpc.Clients;

namespace MediaService.Services;

public class ChunkerCallbackService(IContentGrpcClient contentGrpcClient) : IChunkerCallbackService
{
    public async Task OnSuccess(ProcessVideoDtoCallbackSuccessDto successDto)
    {
        var episodeId = successDto.Key.Split('/', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1) ?? string.Empty;
        var url = $"{successDto.BaseUrl}/{successDto.Key}";

        await contentGrpcClient.SetEpisodeVideoUrl(new EpisodeVideoUrlDto(episodeId, url, FileStatus.Uploaded));
    }

    public async Task OnFailed(ProcessVideoDtoCallbackFailedDto failedDto)
    {
        var episodeId = failedDto.Key.Split('/', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1) ?? string.Empty;

        await contentGrpcClient.SetEpisodeVideoUrl(new EpisodeVideoUrlDto(episodeId, "", FileStatus.Error));
    }
}
