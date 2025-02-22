using System.Diagnostics;

namespace ChunkerService.Utils.ProcessRunners;

public class FfmpegProcessRunner(ILogger<FfmpegProcessRunner> logger) : IProcessRunner
{
    public async Task RunProcessAsync(
        ProcessStartInfo startInfo,
        CancellationToken cancellationToken
    )
    {
        using var process = new Process();
        process.StartInfo = startInfo;

        process.OutputDataReceived += (
            _,
            e
        ) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                logger.LogInformation("[ffmpeg output] {data}", e.Data);
            }
        };

        process.ErrorDataReceived += (
            _,
            e
        ) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                logger.LogInformation("[ffmpeg output] {data}", e.Data);
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
                process.Kill(entireProcessTree: true);
                process.CancelOutputRead();
                process.CancelErrorRead();
                await process.WaitForExitAsync(CancellationToken.None);
                logger.LogInformation("FFmpeg process forcibly terminated due to cancellation");
            }
        }
    }
}
