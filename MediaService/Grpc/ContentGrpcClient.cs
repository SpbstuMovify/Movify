using MediaService.Dtos.Content;
using MediaService.Utils.FileProcessing;
using Movify;

namespace MediaService.Grpc;

public class ContentGrpcClient(ContentService.ContentServiceClient contentServiceClient) : IContentGrpcClient
{
    public async Task SetContentImageUrl(ContentImageUrlDto contentImageUrlDto)
    {
        await contentServiceClient.SetContentImageUrlAsync(new SetContentImageUrlRequest
        {
            ContentId = contentImageUrlDto.ContentId,
            Url = contentImageUrlDto.Url
        });
    }

    public async Task SetEpisodeVideoUrlDto(EpisodeVideoUrlDto episodeVideoUrlDto)
    {
        await contentServiceClient.SetEpisodeVideoUrlAsync(new SetEpisodeVideoUrlRequest
        {
            EpisodeId = episodeVideoUrlDto.EpisodeId,
            Url = episodeVideoUrlDto.Url,
            Status = ParseStatus(episodeVideoUrlDto.Status)
        });
    }

    static string ParseStatus(FileStatus status)
    {
        return status switch
        {
            FileStatus.NotUploaded => "NOT_UPLOADED",
            FileStatus.InProgress => "IN_PROGRESS",
            FileStatus.Error => "ERROR",
            FileStatus.Uploaded => "UPLOADED",
            _ => "UNEXPECTED"
        };
    }
}