using MediaService.Dtos.Content;
using MediaService.FileProcessing;

namespace MediaService.Grpc.Clients;

public class ContentGrpcClient(Movify.ContentService.ContentServiceClient contentServiceClient) : IContentGrpcClient
{
    public async Task SetContentImageUrl(ContentImageUrlDto contentImageUrlDto)
    {
        if (string.IsNullOrWhiteSpace(contentImageUrlDto.ContentId)) throw new ArgumentException("ContentId cannot be null or empty", nameof(contentImageUrlDto));
        if (string.IsNullOrWhiteSpace(contentImageUrlDto.Url)) throw new ArgumentException("Url cannot be null or empty", nameof(contentImageUrlDto));
        
        await contentServiceClient.SetContentImageUrlAsync(
            new Movify.SetContentImageUrlRequest
            {
                ContentId = contentImageUrlDto.ContentId,
                Url = contentImageUrlDto.Url
            }
        );
    }

    public async Task SetEpisodeVideoUrl(EpisodeVideoUrlDto episodeVideoUrlDto)
    {
        if (string.IsNullOrWhiteSpace(episodeVideoUrlDto.EpisodeId)) throw new ArgumentException("EpisodeId cannot be null or empty", nameof(episodeVideoUrlDto));
        
        await contentServiceClient.SetEpisodeVideoUrlAsync(
            new Movify.SetEpisodeVideoUrlRequest
            {
                EpisodeId = episodeVideoUrlDto.EpisodeId,
                Url = episodeVideoUrlDto.Url,
                Status = ParseStatus(episodeVideoUrlDto.Status)
            }
        );
    }

    private static string ParseStatus(FileStatus status) =>
        status switch
        {
            FileStatus.NotUploaded => "NOT_UPLOADED",
            FileStatus.InProgress => "IN_PROGRESS",
            FileStatus.Error => "ERROR",
            FileStatus.Uploaded => "UPLOADED",
            _ => "UNEXPECTED"
        };
}
