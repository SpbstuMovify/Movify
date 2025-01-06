namespace ChunkerService.Hls;

public interface IHlsCreator
{
    /// <summary>
    /// Создаёт HLS-потоки разных качеств и мастер-файл.
    /// </summary>
    /// <param name="ffmpegPath">Путь к ffmpeg.exe (можно указать "ffmpeg", если он в PATH).</param>
    /// <param name="inputFile">Путь к исходному видео файлу.</param>
    /// <param name="outputDirectory">Папка, в которой будут формироваться выходные файлы и плейлисты.</param>
    /// <param name="variants">Список вариантов (качеств) для HLS.</param>
    /// <param name="segmentDuration">Продолжительность сегмента (чанка) в секундах.</param>
    /// <param name="audioBitrate">Битрейт аудио (например, 128000 = 128 kb/s).</param>
    /// <param name="additionalFfmpegArgs">Дополнительные аргументы для ffmpeg.</param>
    Task CreateHlsMasterPlaylistAsync(
        string ffmpegPath,
        string inputFile,
        string outputDirectory,
        List<HlsVariant> variants,
        int segmentDuration = 10,
        int audioBitrate = 128_000,
        string additionalFfmpegArgs = "",
        CancellationToken cancellationToken = default
    );
}