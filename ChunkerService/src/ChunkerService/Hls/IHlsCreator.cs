using ChunkerService.Dtos.Hls;

namespace ChunkerService.Hls;

public interface IHlsCreator
{
    /// <summary>
    /// Создаёт HLS-потоки разных качеств и мастер-файл.
    /// </summary>
    /// <param name="hslParams"></param>
    /// <param name="cancellationToken"></param>
    Task CreateHlsMasterPlaylistAsync(
        HlsParamsDto hslParams,
        CancellationToken cancellationToken
    );

    CancellationToken CreateToken(Guid guid);
    void CancelToken(Guid guid);
}
