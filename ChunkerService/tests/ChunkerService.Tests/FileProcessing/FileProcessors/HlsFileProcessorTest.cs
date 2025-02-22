using ChunkerService.Dtos.Chunker;
using ChunkerService.Dtos.Hls;
using ChunkerService.Dtos.S3;
using ChunkerService.FileProcessing.FileProcessors;
using ChunkerService.Grpc;
using ChunkerService.Hls;
using ChunkerService.Repositories;
using ChunkerService.Services;
using ChunkerService.Utils.Configuration;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace ChunkerService.Tests.FileProcessing.FileProcessors;

[TestSubject(typeof(HlsFileProcessor))]
public class HlsFileProcessorTest
{
    private readonly Mock<IHlsCreator> _hlsCreatorMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly IOptions<HlsOptions> _options;
    private readonly HlsFileProcessor _processor;
    private readonly Mock<IChunkerRepository> _chunkerRepositoryMock;
    private readonly Mock<IMediaGrpcClient> _mediaGrpcClientMock;
    private readonly IServiceScope _serviceScope;

    public HlsFileProcessorTest()
    {
        var loggerMock = new Mock<ILogger<HlsFileProcessor>>();
        _hlsCreatorMock = new Mock<IHlsCreator>();
        _fileServiceMock = new Mock<IFileService>();

        // Настройка опций для HLS
        var hlsOptions = new HlsOptions
        {
            FfmpegPath = "ffmpeg",
            Variants =
            [
                new HlsVariantOptions { Name = "360p", Width = 640, Height = 360, VideoBitrate = 800000 }
            ],
            SegmentDuration = 10,
            AudioBitrate = 128000,
            AdditionalFfmpegArgs = ""
        };
        _options = Options.Create(hlsOptions);

        // Мок репозитория и gRPC-клиента, которые получаются через scope
        _chunkerRepositoryMock = new Mock<IChunkerRepository>();
        _mediaGrpcClientMock = new Mock<IMediaGrpcClient>();

        // Мок ServiceProvider, возвращающего необходимые сервисы
        var serviceProviderMock = new Mock<IServiceProvider>();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IChunkerRepository)))
            .Returns(_chunkerRepositoryMock.Object);
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IMediaGrpcClient)))
            .Returns(_mediaGrpcClientMock.Object);

        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.SetupGet(s => s.ServiceProvider).Returns(serviceProviderMock.Object);
        _serviceScope = serviceScopeMock.Object;

        _processor = new HlsFileProcessor(
            loggerMock.Object,
            _hlsCreatorMock.Object,
            _fileServiceMock.Object,
            _options
        );
    }

    [Fact]
    public async Task ProcessAsync_SuccessfulProcessing()
    {
        // Arrange
        var request = new FileProcessorRequest("temp", "test-bucket", "videos/video.mp4", "http://example.com");

        // Мокаем загрузку файла
        var fileContent = new MemoryStream("file content"u8.ToArray());
        var fileToProcess = new FileData(fileContent, "video/mp4", "video.mp4");
        _chunkerRepositoryMock
            .Setup(r => r.DownloadFileAsync(request.BucketName, request.Key))
            .ReturnsAsync(fileToProcess);

        // Мокаем создание файла для записи
        var fileCreationStream = new MemoryStream();
        _fileServiceMock
            .Setup(fs => fs.CreateFile(It.IsAny<string>()))
            .Returns(fileCreationStream);

        // Мокаем создание токена отмены
        var hlsToken = CancellationToken.None;
        var capturedGuid = Guid.Empty;
        _hlsCreatorMock
            .Setup(h => h.CreateToken(It.IsAny<Guid>()))
            .Callback<Guid>(g => capturedGuid = g)
            .Returns(hlsToken);

        // Мокаем успешное создание HLS-мастер-плейлиста
        _hlsCreatorMock
            .Setup(h => h.CreateHlsMasterPlaylistAsync(It.IsAny<HlsParamsDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Мокаем файлы, которые должны быть загружены после создания HLS-потоков
        var hlsStreamPath = Path.Combine(request.Path, "hls");
        var filePaths = new[]
        {
            Path.Combine(hlsStreamPath, "master.m3u8"),
            Path.Combine(hlsStreamPath, "segment.ts"),
            Path.Combine(hlsStreamPath, "other.txt")
        };
        _fileServiceMock
            .Setup(fs => fs.GetFiles(hlsStreamPath))
            .Returns(filePaths);

        // Мокаем открытие файлов для чтения
        _fileServiceMock
            .Setup(fs => fs.OpenReadFile(It.IsAny<string>()))
            .Returns(new MemoryStream("dummy content"u8.ToArray()));

        // Мокаем загрузку файлов в репозиторий
        _chunkerRepositoryMock
            .Setup(r => r.UploadFileAsync(It.IsAny<FileData>(), request.BucketName, It.IsAny<string>()))
            .ReturnsAsync(new S3ObjectDto(request.BucketName, "http://presigned.url"));

        // Мокаем callback для успешной обработки видео
        _mediaGrpcClientMock
            .Setup(m => m.ProcessVideoCallback(It.IsAny<ProcessVideoCallbackDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _processor.ProcessAsync(request, _serviceScope, hlsToken);

        // Assert

        // Проверяем, что CreateToken и CancelToken вызываются
        _hlsCreatorMock.Verify(h => h.CreateToken(It.IsAny<Guid>()), Times.Once);
        _hlsCreatorMock.Verify(h => h.CancelToken(capturedGuid), Times.Once);

        // Проверяем, что CreateHlsMasterPlaylistAsync вызывается с ожидаемыми параметрами
        _hlsCreatorMock.Verify(
            h => h.CreateHlsMasterPlaylistAsync(
                It.Is<HlsParamsDto>(
                    p =>
                        p.FfmpegPath == _options.Value.FfmpegPath &&
                        p.InputFile == Path.Combine(request.Path, fileToProcess.FileName) &&
                        p.OutputDirectory == hlsStreamPath &&
                        p.SegmentDuration == _options.Value.SegmentDuration &&
                        p.AudioBitrate == _options.Value.AudioBitrate &&
                        p.AdditionalFfmpegArgs == _options.Value.AdditionalFfmpegArgs &&
                        p.Variants.Count == _options.Value.Variants.Length
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        // Проверяем, что UploadFileAsync вызывается для каждого файла с корректными ключами и content-type
        var prefix = request.Key[..request.Key.LastIndexOf('/')];
        _chunkerRepositoryMock.Verify(
            r => r.UploadFileAsync(
                It.Is<FileData>(f => f.ContentType == "application/vnd.apple.mpegurl" && f.FileName == "master.m3u8"),
                request.BucketName,
                $"{prefix}/hls/master.m3u8"
            ),
            Times.Once
        );
        _chunkerRepositoryMock.Verify(
            r => r.UploadFileAsync(
                It.Is<FileData>(f => f.ContentType == "video/mp2t" && f.FileName == "segment.ts"),
                request.BucketName,
                $"{prefix}/hls/segment.ts"
            ),
            Times.Once
        );
        _chunkerRepositoryMock.Verify(
            r => r.UploadFileAsync(
                It.Is<FileData>(f => f.ContentType == "application/octet-stream" && f.FileName == "other.txt"),
                request.BucketName,
                $"{prefix}/hls/other.txt"
            ),
            Times.Once
        );

        // Проверяем, что вызывается callback с успешным masterKey
        _mediaGrpcClientMock.Verify(
            m => m.ProcessVideoCallback(
                It.Is<ProcessVideoCallbackDto>(
                    dto =>
                        dto.BucketName == request.BucketName &&
                        dto.Key == $"{prefix}/hls/master.m3u8" &&
                        dto.BaseUrl == request.BaseUrl &&
                        dto.Error == null
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task ProcessAsync_WhenExceptionOccurs_CallsProcessVideoCallbackWithError()
    {
        // Arrange
        var request = new FileProcessorRequest("temp", "test-bucket", "videos/video.mp4", "http://example.com");

        // Мокаем загрузку файла
        var fileContent = new MemoryStream("file content"u8.ToArray());
        var fileToProcess = new FileData(fileContent, "video/mp4", "video.mp4");
        _chunkerRepositoryMock
            .Setup(r => r.DownloadFileAsync(request.BucketName, request.Key))
            .ReturnsAsync(fileToProcess);

        // Мокаем создание файла для записи
        var fileCreationStream = new MemoryStream();
        _fileServiceMock
            .Setup(fs => fs.CreateFile(It.IsAny<string>()))
            .Returns(fileCreationStream);

        // Мокаем создание токена отмены
        var hlsToken = CancellationToken.None;
        var capturedGuid = Guid.Empty;
        _hlsCreatorMock
            .Setup(h => h.CreateToken(It.IsAny<Guid>()))
            .Callback<Guid>(g => capturedGuid = g)
            .Returns(hlsToken);

        // Мокаем метод создания HLS-плейлиста, выбрасывающий исключение
        const string exceptionMessage = "Test exception";
        _hlsCreatorMock
            .Setup(h => h.CreateHlsMasterPlaylistAsync(It.IsAny<HlsParamsDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Мокаем callback для обработки ошибки
        _mediaGrpcClientMock
            .Setup(m => m.ProcessVideoCallback(It.IsAny<ProcessVideoCallbackDto>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _processor.ProcessAsync(request, _serviceScope, hlsToken));
        Assert.Equal(exceptionMessage, ex.Message);

        // Проверяем, что в callback передается исходный ключ и текст ошибки
        _mediaGrpcClientMock.Verify(
            m => m.ProcessVideoCallback(
                It.Is<ProcessVideoCallbackDto>(
                    dto =>
                        dto.BucketName == request.BucketName &&
                        dto.Key == request.Key &&
                        dto.BaseUrl == null &&
                        dto.Error != null &&
                        dto.Error.Contains(exceptionMessage)
                )
            ),
            Times.Once
        );

        // Проверяем, что CancelToken вызывается в блоке finally
        _hlsCreatorMock.Verify(h => h.CancelToken(capturedGuid), Times.Once);
    }
}
