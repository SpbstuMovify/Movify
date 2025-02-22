using System.Diagnostics;
using System.Runtime.InteropServices;

using ChunkerService.Utils.ProcessRunners;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Moq;

namespace ChunkerService.Tests.Utils.ProcessRunners;

[TestSubject(typeof(FfmpegProcessRunner))]
public class FfmpegProcessRunnerTest
{
    /// <summary>
    /// Проверяет, что если процесс выводит данные в стандартный поток вывода, они логируются.
    /// </summary>
    [Fact]
    public async Task RunProcessAsync_ShouldLogStandardOutput()
    {
        // Arrange: создаём мок-логгера и экземпляр класса
        var loggerMock = new Mock<ILogger<FfmpegProcessRunner>>();
        var processRunner = new FfmpegProcessRunner(loggerMock.Object);

        ProcessStartInfo startInfo;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // На Windows команда echo через cmd.exe
            startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c echo test",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }
        else
        {
            // На Linux/macOS используем команду echo
            startInfo = new ProcessStartInfo
            {
                FileName = "echo",
                Arguments = "test",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        var cancellationToken = CancellationToken.None;

        // Act: запускаем процесс
        await processRunner.RunProcessAsync(startInfo, cancellationToken);

        // Assert: убеждаемся, что логгер получил вызов с текстом "test"
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>(
                    (
                        v,
                        t
                    ) => v.ToString()!.Contains("test", StringComparison.OrdinalIgnoreCase)
                ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!
            ),
            Times.AtLeastOnce()
        );
    }

    /// <summary>
    /// Проверяет, что если процесс выводит данные в стандартный поток ошибок, они также логируются.
    /// </summary>
    [Fact]
    public async Task RunProcessAsync_ShouldLogStandardError()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<FfmpegProcessRunner>>();
        var processRunner = new FfmpegProcessRunner(loggerMock.Object);

        ProcessStartInfo startInfo;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // На Windows: вывод в stderr через перенаправление (используем конструкцию cmd.exe)
            startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                // '>&2' перенаправляет вывод в stderr (учтите, что синтаксис может варьироваться)
                Arguments = "/c echo test 1>&2",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }
        else
        {
            // На Linux/macOS: через sh -c и перенаправление вывода в stderr
            startInfo = new ProcessStartInfo
            {
                FileName = "sh",
                Arguments = "-c \"echo test 1>&2\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        var cancellationToken = CancellationToken.None;

        // Act
        await processRunner.RunProcessAsync(startInfo, cancellationToken);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>(
                    (
                        v,
                        t
                    ) => v.ToString()!.Contains("test", StringComparison.OrdinalIgnoreCase)
                ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!
            ),
            Times.AtLeastOnce()
        );
    }

    /// <summary>
    /// Проверяет, что при отмене выполнения процесса вызывается его принудительное завершение
    /// и логируется сообщение о принудительной остановке.
    /// </summary>
    [Fact]
    public async Task RunProcessAsync_ShouldTerminateProcessOnCancellation()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<FfmpegProcessRunner>>();
        var processRunner = new FfmpegProcessRunner(loggerMock.Object);

        ProcessStartInfo startInfo;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // На Windows: команда ping с большим количеством повторов для имитации долгого процесса.
            startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c ping 127.0.0.1 -n 1000",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }
        else
        {
            // На Linux/macOS: используем команду sleep на большое время
            startInfo = new ProcessStartInfo
            {
                FileName = "sleep",
                Arguments = "1000",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        // Устанавливаем токен отмены, который сработает через 100 мс
        using var cts = new CancellationTokenSource(100);

        // Act
        await processRunner.RunProcessAsync(startInfo, cts.Token);

        // Assert: убеждаемся, что логгер получил сообщение о принудительном завершении процесса
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>(
                    (
                            v,
                            t
                        ) =>
                        v.ToString()!.Contains("FFmpeg process forcibly terminated due to cancellation", StringComparison.OrdinalIgnoreCase)
                ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!
            ),
            Times.Once()
        );
    }
}
