using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

using ChunkerService.Dtos.Hls;
using ChunkerService.Services;
using ChunkerService.Utils.ProcessRunners;

namespace ChunkerService.Hls;

public class HlsCreator(
    ILogger<HlsCreator> logger,
    IProcessRunner processRunner,
    IFileService fileService
) : IHlsCreator
{
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _cancellationTokens = new();

    /// <summary>
    /// Создаёт HLS-потоки разных качеств и мастер-файл.
    /// </summary>
    /// <param name="hslParams"></param>
    /// <param name="cancellationToken"></param>
    public async Task CreateHlsMasterPlaylistAsync(
        HlsParamsDto hslParams,
        CancellationToken cancellationToken
    )
    {
        if (!fileService.DirectoryExists(hslParams.OutputDirectory))
        {
            fileService.CreateDirectory(hslParams.OutputDirectory);
        }

        var generatedPlaylists = new List<string>();
        var ffmpegTasks = new List<Task>();

        var availableThreads = Environment.ProcessorCount / hslParams.Variants.Count;
        var optimalThreads = Math.Max(1, availableThreads);

        logger.LogInformation($"Threads per task[{optimalThreads}]");

        foreach (var variant in hslParams.Variants)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Task was cancelled before starting ffmpeg");
                return;
            }

            var playlistFileName = $"{variant.Name}.m3u8";
            var playlistFullPath = Path.Combine(hslParams.OutputDirectory, playlistFileName);
            var segmentFileNameTemplate = Path.Combine(hslParams.OutputDirectory, $"{variant.Name}_%03d.ts");

            // ffmpeg:
            //   -vf scale=w:h          — изменяем размер кадра.
            //   -b:v X                 — битрейт для видео.
            //   -b:a X                 — битрейт для аудио.
            //   -hls_time X            — длина сегмента в секундах.
            //   -hls_playlist_type vod — ffmpeg укажет, что это VoD-плейлист.
            //   -hls_segment_filename  — шаблон названий сегментов.
            //   -f hls                 — формат вывода (HLS).
            //   {variant.Name}.m3u8    — конечный плейлист.
            var arguments =
                $"-loglevel info " +
                $"-i \"{hslParams.InputFile}\" " +
                $"-vf scale={variant.Width}:{variant.Height} " +
                $"-c:v h264 -b:v {variant.VideoBitrate} -preset veryfast " +
                $"-threads {optimalThreads} " +
                $"-c:a aac -b:a {hslParams.AudioBitrate} -ac 2 " +
                $"{hslParams.AdditionalFfmpegArgs} " +
                $"-hls_time {hslParams.SegmentDuration} " +
                $"-hls_playlist_type vod " +
                $"-hls_segment_filename \"{segmentFileNameTemplate}\" " +
                $"-f hls \"{playlistFullPath}\"";

            var startInfo = new ProcessStartInfo
            {
                FileName = hslParams.FfmpegPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            ffmpegTasks.Add(processRunner.RunProcessAsync(startInfo, cancellationToken));
            generatedPlaylists.Add(playlistFileName);
        }

        await Task.WhenAll(ffmpegTasks);

        if (!cancellationToken.IsCancellationRequested)
        {
            await CreateMasterM3U8Async(hslParams.OutputDirectory, generatedPlaylists, hslParams.Variants, cancellationToken);
        }
    }

    /// <summary>
    /// Формирует общий master.m3u8, который ссылается на все m3u8-файлы разных качеств.
    /// </summary>
    /// <param name="outputDirectory">Папка, где будет создаваться master.m3u8.</param>
    /// <param name="playlists">Список файлов плейлистов (например, "360p.m3u8" и т.д.).</param>
    /// <param name="variants">Список вариантов (качеств), соответствующих плейлистам.</param>
    /// <param name="cancellationToken"></param>
    private async Task CreateMasterM3U8Async(
        string outputDirectory,
        List<string> playlists,
        List<HlsVariantDto> variants,
        CancellationToken cancellationToken
    )
    {
        if (playlists.Count != variants.Count)
        {
            throw new ArgumentException("Playlist count mismatch");
        }

        var masterFilePath = Path.Combine(outputDirectory, "master.m3u8");

        var sb = new StringBuilder();
        sb.AppendLine("#EXTM3U");
        sb.AppendLine("#EXT-X-VERSION:3");

        for (var i = 0; i < playlists.Count; i++)
        {
            var variant = variants[i];
            sb.AppendLine(
                $"#EXT-X-STREAM-INF:PROGRAM-ID=1," +
                $"BANDWIDTH={variant.VideoBitrate}," +
                $"RESOLUTION={variant.Width}x{variant.Height}"
            );
            sb.AppendLine(playlists[i]);
        }

        try
        {
            await fileService.WriteAllTextAsync(masterFilePath, sb.ToString(), cancellationToken);
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Create master m3u8 forcibly terminated due to cancellation");
        }
    }

    public CancellationToken CreateToken(Guid guid)
    {
        _cancellationTokens[guid] = new CancellationTokenSource();
        return _cancellationTokens[guid].Token;
    }

    public void CancelToken(Guid guid)
    {
        if (!_cancellationTokens.TryRemove(guid, out var cts))
        {
            return;
        }

        logger.LogInformation("_cancellationTokens");
        cts.Cancel();
    }
}
