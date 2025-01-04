using MediaService.Dtos.Content;

namespace MediaService.Grpc;

public interface IContentGrpcClient
{
    Task SetContentImageUrl(ContentImageUrlDto contentImageUrlDto);
    Task SetEpisodeVideoUrlDto(EpisodeVideoUrlDto episodeVideoUrlDto);
}