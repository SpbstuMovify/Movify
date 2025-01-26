using MediaService.Dtos.Content;
using MediaService.Utils.FileProcessing;

namespace MediaService.Grpc;

public class ContentGrpcClient(Movify.ContentService.ContentServiceClient contentServiceClient) : IContentGrpcClient
{
    public async Task SetContentImageUrl(ContentImageUrlDto contentImageUrlDto)
    {
        await contentServiceClient.SetContentImageUrlAsync(new Movify.SetContentImageUrlRequest
        {
            ContentId = contentImageUrlDto.ContentId,
            Url = contentImageUrlDto.Url
        });
    }

    public async Task SetEpisodeVideoUrlDto(EpisodeVideoUrlDto episodeVideoUrlDto)
    {
        await contentServiceClient.SetEpisodeVideoUrlAsync(new Movify.SetEpisodeVideoUrlRequest
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
