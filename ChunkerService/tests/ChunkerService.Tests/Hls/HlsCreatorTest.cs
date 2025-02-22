using System.Diagnostics;

using ChunkerService.Dtos.Hls;
using ChunkerService.Hls;
using ChunkerService.Services;
using ChunkerService.Utils.ProcessRunners;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Moq;

namespace ChunkerService.Tests.Hls;

[TestSubject(typeof(HlsCreator))]
public class HlsCreatorTest
{
    private readonly Mock<IProcessRunner> _processRunnerMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly HlsCreator _hlsCreator;

    public HlsCreatorTest()
    {
        var loggerMock = new Mock<ILogger<HlsCreator>>();
        _processRunnerMock = new Mock<IProcessRunner>();
        _fileServiceMock = new Mock<IFileService>();
        _hlsCreator = new HlsCreator(loggerMock.Object, _processRunnerMock.Object, _fileServiceMock.Object);
    }

    // Вспомогательный метод для создания HlsParamsDto с указанным числом вариантов
    private static HlsParamsDto CreateSampleHlsParamsDto(int variantCount = 1)
    {
        var variants = new List<HlsVariantDto>();
        for (var i = 0; i < variantCount; i++)
        {
            // Название вида "variant1", "variant2", и т.д.
            variants.Add(new HlsVariantDto($"variant{i + 1}", 640 * (i + 1), 360 * (i + 1), 800000 * (i + 1)));
        }

        return new HlsParamsDto("ffmpeg", "input.mp4", "output", variants, 10, 128000, "");
    }

    [Fact]
    public async Task CreateHlsMasterPlaylistAsync_CreatesDirectoryIfNotExists()
    {
        // Arrange
        var hlsParams = CreateSampleHlsParamsDto(2);
        var cts = new CancellationTokenSource();
        _fileServiceMock.Setup(fs => fs.DirectoryExists(hlsParams.OutputDirectory)).Returns(false);

        _processRunnerMock
            .Setup(pr => pr.RunProcessAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _fileServiceMock
            .Setup(fs => fs.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _hlsCreator.CreateHlsMasterPlaylistAsync(hlsParams, cts.Token);

        // Assert: если каталог не существует, должен быть вызов CreateDirectory
        _fileServiceMock.Verify(fs => fs.CreateDirectory(hlsParams.OutputDirectory), Times.Once);
    }

    [Fact]
    public async Task CreateHlsMasterPlaylistAsync_CallsProcessRunnerForEachVariant()
    {
        // Arrange
        const int variantCount = 2;
        var hlsParams = CreateSampleHlsParamsDto(variantCount);
        var cts = new CancellationTokenSource();
        _fileServiceMock.Setup(fs => fs.DirectoryExists(hlsParams.OutputDirectory)).Returns(true);

        var optimalThreads = Math.Max(1, Environment.ProcessorCount / hlsParams.Variants.Count);

        var processStartInfos = new List<ProcessStartInfo>();
        _processRunnerMock
            .Setup(pr => pr.RunProcessAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()))
            .Callback<ProcessStartInfo, CancellationToken>(
                (
                    psi,
                    _
                ) =>
                {
                    processStartInfos.Add(psi);
                }
            )
            .Returns(Task.CompletedTask);

        _fileServiceMock
            .Setup(fs => fs.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _hlsCreator.CreateHlsMasterPlaylistAsync(hlsParams, cts.Token);

        // Assert: для каждого варианта должен быть вызван процесс ffmpeg
        _processRunnerMock.Verify(pr => pr.RunProcessAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()), Times.Exactly(variantCount));
        Assert.Equal(variantCount, processStartInfos.Count);
        foreach (var variant in hlsParams.Variants)
        {
            var expectedPlaylist = $"{variant.Name}.m3u8";
            Assert.Contains(
                processStartInfos,
                psi =>
                    psi.FileName == hlsParams.FfmpegPath &&
                    psi.Arguments.Contains($"-i \"{hlsParams.InputFile}\"") &&
                    psi.Arguments.Contains($"-vf scale={variant.Width}:{variant.Height}") &&
                    psi.Arguments.Contains($"-b:v {variant.VideoBitrate}") &&
                    psi.Arguments.Contains($"-threads {optimalThreads}") &&
                    psi.Arguments.Contains(expectedPlaylist)
            );
        }
    }

    [Fact]
    public async Task CreateHlsMasterPlaylistAsync_DoesNotCreateMasterPlaylistIfCancelledBeforeProcessStart()
    {
        // Arrange
        var hlsParams = CreateSampleHlsParamsDto();
        var cts = new CancellationTokenSource();
        await cts.CancelAsync(); // Отмена до начала выполнения

        // Act
        await _hlsCreator.CreateHlsMasterPlaylistAsync(hlsParams, cts.Token);

        // Assert: если токен отменён, ни ffmpeg, ни запись master.m3u8 не выполняются
        _processRunnerMock.Verify(pr => pr.RunProcessAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateHlsMasterPlaylistAsync_HandlesTaskCanceledExceptionWhenCreatingMasterPlaylist()
    {
        // Arrange
        var hlsParams = CreateSampleHlsParamsDto();
        var cts = new CancellationTokenSource();
        _fileServiceMock.Setup(fs => fs.DirectoryExists(hlsParams.OutputDirectory)).Returns(true);

        _processRunnerMock
            .Setup(pr => pr.RunProcessAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Имитируем выброс TaskCanceledException при записи master.m3u8
        _fileServiceMock
            .Setup(fs => fs.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TaskCanceledException());

        // Act & Assert: исключение не должно быть проброшено наружу
        var exception = await Record.ExceptionAsync(() => _hlsCreator.CreateHlsMasterPlaylistAsync(hlsParams, cts.Token));
        Assert.Null(exception);
    }

    [Fact]
    public void CreateTokenAndCancelToken_WorksCorrectly()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var token = _hlsCreator.CreateToken(guid);
        Assert.False(token.IsCancellationRequested);

        _hlsCreator.CancelToken(guid);
        Assert.True(token.IsCancellationRequested);
    }

    [Fact]
    public void CancelToken_DoesNotThrow_WhenTokenNotFound()
    {
        // Act & Assert: если токен с данным Guid не найден, метод не должен выбрасывать исключение
        var nonExistentGuid = Guid.NewGuid();
        var exception = Record.Exception(() => _hlsCreator.CancelToken(nonExistentGuid));
        Assert.Null(exception);
    }

    [Fact]
    public async Task CreateHlsMasterPlaylistAsync_CreatesMasterPlaylistWithCorrectContent()
    {
        // Arrange
        const int variantCount = 2;
        var hlsParams = CreateSampleHlsParamsDto(variantCount);
        var cts = new CancellationTokenSource();
        _fileServiceMock.Setup(fs => fs.DirectoryExists(hlsParams.OutputDirectory)).Returns(true);

        // Завершаем задачи ffmpeg мгновенно
        _processRunnerMock
            .Setup(pr => pr.RunProcessAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        string? masterPlaylistContent = null;
        _fileServiceMock
            .Setup(fs => fs.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, CancellationToken>(
                (
                    _,
                    content,
                    _
                ) =>
                {
                    masterPlaylistContent = content;
                }
            )
            .Returns(Task.CompletedTask);

        // Act
        await _hlsCreator.CreateHlsMasterPlaylistAsync(hlsParams, cts.Token);

        // Assert: master.m3u8 должен содержать стандартные заголовки и ссылки для каждого варианта
        Assert.NotNull(masterPlaylistContent);
        Assert.Contains("#EXTM3U", masterPlaylistContent);
        Assert.Contains("#EXT-X-VERSION:3", masterPlaylistContent);
        foreach (var variant in hlsParams.Variants)
        {
            Assert.Contains($"BANDWIDTH={variant.VideoBitrate}", masterPlaylistContent);
            Assert.Contains($"RESOLUTION={variant.Width}x{variant.Height}", masterPlaylistContent);
            Assert.Contains($"{variant.Name}.m3u8", masterPlaylistContent);
        }
    }
}
