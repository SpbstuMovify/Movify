using MediaService.Dtos.Content;

namespace MediaService.Grpc.Clients;

public interface IContentGrpcClient
{
    Task SetContentImageUrl(ContentImageUrlDto contentImageUrlDto);
    Task SetEpisodeVideoUrl(EpisodeVideoUrlDto episodeVideoUrlDto);
}
