using MediaService.Dtos.Chunker;
using MediaService.Dtos.Content;
using MediaService.FileProcessing;
using MediaService.Grpc.Clients;

namespace MediaService.Services;

public class ChunkerCallbackService(IContentGrpcClient contentGrpcClient) : IChunkerCallbackService
{
    public async Task OnSuccess(ProcessVideoCallbackSuccessDto successCallbackSuccessDto)
    {
        var episodeId = successCallbackSuccessDto.Key.Split('/', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1) ?? string.Empty;
        var url = $"{successCallbackSuccessDto.BaseUrl}/{successCallbackSuccessDto.Key}";

        await contentGrpcClient.SetEpisodeVideoUrl(new EpisodeVideoUrlDto(episodeId, url, FileStatus.Uploaded));
    }

    public async Task OnFailed(ProcessVideoCallbackFailedDto failedCallbackFailedDto)
    {
        var episodeId = failedCallbackFailedDto.Key.Split('/', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1) ?? string.Empty;

        await contentGrpcClient.SetEpisodeVideoUrl(new EpisodeVideoUrlDto(episodeId, "", FileStatus.Error));
    }
}
