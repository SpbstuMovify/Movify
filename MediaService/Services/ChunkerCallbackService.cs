using MediaService.Dtos.Chunker;
using MediaService.Dtos.Content;
using MediaService.Grpc;
using MediaService.Utils.FileProcessing;

namespace MediaService.Services;

public class ChunkerCallbackService(IContentGrpcClient contentGrpcClient) : IChunkerCallbackService
{
    public async Task OnSuccess(ProcessVideoDtoCallbackSuccessDto successDto)
    {
        string episodeId = ParseSegment(successDto.Key, 1);
        string url = $"{successDto.BaseUrl}/{successDto.Key}";

        await contentGrpcClient.SetEpisodeVideoUrlDto(new EpisodeVideoUrlDto
        {
            EpisodeId = episodeId,
            Url = url,
            Status = FileStatus.Uploaded
        });
    }

    public async Task OnFailed(ProcessVideoDtoCallbackFailedDto failedDto)
    {
        string episodeId = ParseSegment(failedDto.Key, 1);

        await contentGrpcClient.SetEpisodeVideoUrlDto(new EpisodeVideoUrlDto
        {
            EpisodeId = episodeId,
            Url = "",
            Status = FileStatus.Error
        });
    }

    private static string ParseSegment(string path, int index)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        string[] parts = path.Split('/');
        if (index < 0 || index >= parts.Length)
            return string.Empty;

        return parts[index];
    }
}
