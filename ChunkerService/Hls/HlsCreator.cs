using System.Diagnostics;
using System.Text;

namespace ChunkerService.Hls;

public class HlsCreator(ILogger<HlsCreator> logger) : IHlsCreator
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
    public async Task CreateHlsMasterPlaylistAsync(
        string ffmpegPath,
        string inputFile,
        string outputDirectory,
        List<HlsVariant> variants,
        int segmentDuration = 10,
        int audioBitrate = 128_000,
        string additionalFfmpegArgs = "",
        CancellationToken cancellationToken = default
    )
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var generatedPlaylists = new List<string>();

        var ffmpegTasks = new List<Task>();

        int availableThreads = Environment.ProcessorCount / variants.Count;
        int optimalThreads = Math.Max(1, availableThreads);

        logger.LogInformation($"Threads per task[{optimalThreads}]");

        foreach (var variant in variants)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Task was cancelled before starting ffmpeg");
                return;
            }

            string playlistFileName = $"{variant.Name}.m3u8";
            string playlistFullPath = Path.Combine(outputDirectory, playlistFileName);

            string segmentFileNameTemplate = Path.Combine(outputDirectory, $"{variant.Name}_%03d.ts");

            // ffmpeg:
            //   -vf scale=w:h          — изменяем размер кадра.
            //   -b:v X                 — битрейт для видео.
            //   -b:a X                 — битрейт для аудио.
            //   -hls_time X            — длина сегмента в секундах.
            //   -hls_playlist_type vod — ffmpeg укажет, что это VoD-плейлист.
            //   -hls_segment_filename  — шаблон названий сегментов.
            //   -f hls                 — формат вывода (HLS).
            //   {variant.Name}.m3u8    — конечный плейлист.
            string arguments =
                $"-i \"{inputFile}\" " +
                $"-vf scale={variant.Width}:{variant.Height} " +
                $"-c:v h264 -b:v {variant.VideoBitrate} -preset veryfast " +
                $"-threads {optimalThreads} " +
                $"-c:a aac -b:a {audioBitrate} -ac 2 " +
                $"{additionalFfmpegArgs} " +
                $"-hls_time {segmentDuration} " +
                $"-hls_playlist_type vod " +
                $"-hls_segment_filename \"{segmentFileNameTemplate}\" " +
                $"-f hls \"{playlistFullPath}\"";

            ffmpegTasks.Add(RunFfmpegAsync(ffmpegPath, arguments, cancellationToken));

            generatedPlaylists.Add(playlistFileName);
        }

        // Создаём общий мастер-файл master.m3u8
        await Task.WhenAll(ffmpegTasks);

        if (!cancellationToken.IsCancellationRequested)
        {
            await CreateMasterM3u8Async(outputDirectory, generatedPlaylists, variants, cancellationToken);
        }
    }

    /// <summary>
    /// Формирует общий master.m3u8, который ссылается на все m3u8-файлы разных качеств.
    /// </summary>
    /// <param name="outputDirectory">Папка, где будет создаваться master.m3u8.</param>
    /// <param name="playlists">Список файлов плейлистов (например, "360p.m3u8" и т.д.).</param>
    /// <param name="variants">Список вариантов (качеств), соответствующих плейлистам.</param>
    private async Task CreateMasterM3u8Async(string outputDirectory, List<string> playlists, List<HlsVariant> variants, CancellationToken cancellationToken)
    {
        string masterFilePath = Path.Combine(outputDirectory, "master.m3u8");

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("#EXTM3U");
        sb.AppendLine("#EXT-X-VERSION:3");

        for (int i = 0; i < playlists.Count; i++)
        {
            var variant = variants[i];
            sb.AppendLine(
                $"#EXT-X-STREAM-INF:PROGRAM-ID=1," +
                $"BANDWIDTH={variant.VideoBitrate}," +
                $"RESOLUTION={variant.Width}x{variant.Height}"
            );
            sb.AppendLine(playlists[i]);
        }

        await File.WriteAllTextAsync(masterFilePath, sb.ToString(), cancellationToken);
    }

    /// <summary>
    /// Запускает ffmpeg с указанными аргументами.
    /// </summary>
    /// <param name="ffmpegPath">Путь к ffmpeg.exe.</param>
    /// <param name="arguments">Аргументы командной строки ffmpeg.</param>
    private async Task RunFfmpegAsync(string ffmpegPath, string arguments, CancellationToken cancellationToken)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    logger.LogInformation($"[ffmpeg output] {e.Data}");
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    logger.LogInformation($"[ffmpeg error] {e.Data}");
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            try
            {
                await process.WaitForExitAsync(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    logger.LogInformation("FFmpeg process forcibly terminated due to cancellation");
                }
            }
        }
    }
}