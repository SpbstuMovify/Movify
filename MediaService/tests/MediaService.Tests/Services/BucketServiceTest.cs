using JetBrains.Annotations;

using MediaService.Dtos.Bucket;
using MediaService.Dtos.FileInfo;
using MediaService.Dtos.S3;
using MediaService.FileProcessing;
using MediaService.Repositories;
using MediaService.Services;
using MediaService.Utils.Exceptions;

using Moq;

namespace MediaService.Tests.Services;

[TestSubject(typeof(BucketService))]
public class BucketServiceTest
{
    private readonly Mock<IBucketRepository> _bucketRepositoryMock;
    private readonly Mock<IFileProcessingQueue> _fileProcessingQueueMock;
    private readonly BucketService _service;

    public BucketServiceTest()
    {
        _bucketRepositoryMock = new Mock<IBucketRepository>();
        _fileProcessingQueueMock = new Mock<IFileProcessingQueue>();

        _service = new BucketService(
            _bucketRepositoryMock.Object,
            _fileProcessingQueueMock.Object
        );
    }

    // ---------------------------------------------------
    // 1) GetBucketsAsync
    // ---------------------------------------------------
    [Fact]
    public async Task GetBucketsAsync_WithSuccess_ReturnsBucketDtos()
    {
        // Arrange
        var buckets = new List<S3BucketDto>
        {
            new("bucketA"),
            new("bucketB")
        };

        _bucketRepositoryMock
            .Setup(repo => repo.GetBucketsAsync())
            .ReturnsAsync(buckets);

        // Act
        var result = await _service.GetBucketsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, b => b.Name == "bucketA");
        Assert.Contains(result, b => b.Name == "bucketB");
    }

    [Fact]
    public async Task GetBucketsAsync_WhenExceptionThrown_ThrowsInternalServerErrorException()
    {
        // Arrange
        _bucketRepositoryMock
            .Setup(repo => repo.GetBucketsAsync())
            .Throws(new InvalidOperationException("Some DB error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InternalServerErrorException>(() => _service.GetBucketsAsync());
        Assert.Contains("Failed to get buckets", ex.Message);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    [Fact]
    public async Task GetBucketsAsync_WhenNotFoundExceptionThrown_RethrowsNotFoundException()
    {
        // Arrange
        _bucketRepositoryMock
            .Setup(repo => repo.GetBucketsAsync())
            .Throws(new NotFoundException("No buckets found"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetBucketsAsync());
        Assert.Equal("No buckets found", ex.Message);
    }

    // ---------------------------------------------------
    // 2) CreateBucketAsync
    // ---------------------------------------------------
    [Fact]
    public async Task CreateBucketAsync_WithSuccess_ReturnsBucketDto()
    {
        // Arrange
        var createDto = new CreateBucketDto("my-new-bucket");
        var bucketEntity = new S3BucketDto("my-new-bucket");

        _bucketRepositoryMock
            .Setup(repo => repo.CreateBucketAsync("my-new-bucket"))
            .ReturnsAsync(bucketEntity);

        // Act
        var result = await _service.CreateBucketAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("my-new-bucket", result.Name);
    }

    [Fact]
    public async Task CreateBucketAsync_WhenExceptionThrown_ThrowsInternalServerErrorException()
    {
        // Arrange
        var createDto = new CreateBucketDto("bucketX");
        _bucketRepositoryMock
            .Setup(repo => repo.CreateBucketAsync("bucketX"))
            .Throws(new InvalidOperationException("fail create"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InternalServerErrorException>(() => _service.CreateBucketAsync(createDto));
        Assert.Contains("Failed to create bucket", ex.Message);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    // ---------------------------------------------------
    // 3) DeleteBucketAsync
    // ---------------------------------------------------
    [Fact]
    public async Task DeleteBucketAsync_WithSuccess_NoExceptions()
    {
        // Arrange
        var deleteDto = new DeleteBucketDto("old-bucket");

        _bucketRepositoryMock
            .Setup(repo => repo.DeleteBucketAsync("old-bucket"))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteBucketAsync(deleteDto);

        // Assert
        _bucketRepositoryMock.Verify(
            x => x.DeleteBucketAsync("old-bucket"),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteBucketAsync_WhenExceptionThrown_ThrowsInternalServerErrorException()
    {
        // Arrange
        var deleteDto = new DeleteBucketDto("bad-bucket");
        _bucketRepositoryMock
            .Setup(repo => repo.DeleteBucketAsync("bad-bucket"))
            .Throws(new InvalidOperationException("delete failed"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InternalServerErrorException>(() => _service.DeleteBucketAsync(deleteDto));
        Assert.Contains("Failed to delete bucket", ex.Message);
    }

    // ---------------------------------------------------
    // 4) GetFilesAsync
    // ---------------------------------------------------
    [Fact]
    public async Task GetFilesAsync_WithSuccess_ReturnsFileInfoDtos()
    {
        // Arrange
        var getFilesDto = new GetFilesInfoDto("bucket123", "prefixABC");
        var s3Objects = new List<S3ObjectDto>
        {
            new S3ObjectDto("bucket123", "http://url1"),
            new S3ObjectDto("bucket123", "http://url2")
        };

        _bucketRepositoryMock
            .Setup(repo => repo.GetFilesAsync("bucket123", "prefixABC"))
            .ReturnsAsync(s3Objects);

        // Act
        var result = await _service.GetFilesAsync(getFilesDto);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f is { BucketName: "bucket123", PresignedUrl: "http://url1" });
        Assert.Contains(result, f => f is { BucketName: "bucket123", PresignedUrl: "http://url2" });
    }

    [Fact]
    public async Task GetFilesAsync_WhenExceptionThrown_ThrowsInternalServerErrorException()
    {
        // Arrange
        var getFilesDto = new GetFilesInfoDto("bckt", "pre");
        _bucketRepositoryMock
            .Setup(repo => repo.GetFilesAsync("bckt", "pre"))
            .Throws(new ApplicationException("Get files failure"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InternalServerErrorException>(() => _service.GetFilesAsync(getFilesDto));
        Assert.Contains("Failed to get files", ex.Message);
        Assert.IsType<ApplicationException>(ex.InnerException);
    }

    // ---------------------------------------------------
    // 5) CreateFile
    // ---------------------------------------------------
    [Fact]
    public void CreateFile_WithPrefix_EnqueuesTaskAndReturnsProcessingInfo()
    {
        // Arrange
        var uploadedFile = new UploadedFileInfoDto("local/path/file.mp4", "video/mp4", "file.mp4");
        var dto = new CreateFileInfoDto(
            uploadedFile,
            "video-bucket",
            "series/ep1",
            true,
            FileDestination.EpisodeVideoUrl,
            "http://cdn.com"
        );

        // Act
        var result = _service.CreateFile(dto);

        // Assert
        _fileProcessingQueueMock.Verify(
            q => q.Enqueue(
                It.Is<FileProcessingTask>(
                    t =>
                        t.File == uploadedFile &&
                        t.BucketName == "video-bucket" &&
                        t.Key == "series/ep1/file.mp4" &&
                        t.IsVideoProcNecessary == true &&
                        t.Destination == FileDestination.EpisodeVideoUrl &&
                        t.BaseUrl == "http://cdn.com"
                )
            ),
            Times.Once
        );

        Assert.Equal("video-bucket", result.BucketName);
        Assert.Equal("Processing...", result.PresignedUrl);
    }

    [Fact]
    public void CreateFile_WithoutPrefix_KeyIsJustFileName()
    {
        // Arrange
        var uploadedFile = new UploadedFileInfoDto("some/local/file.png", "image/png", "file.png");
        var dto = new CreateFileInfoDto(
            uploadedFile,
            "image-bucket",
            "", // no prefix
            false,
            FileDestination.Internal,
            "http://example.com"
        );

        // Act
        var result = _service.CreateFile(dto);

        // Assert
        _fileProcessingQueueMock.Verify(
            q => q.Enqueue(
                It.Is<FileProcessingTask>(
                    t =>
                        t.Key == "file.png"
                )
            ),
            Times.Once
        );

        Assert.Equal("image-bucket", result.BucketName);
        Assert.Equal("Processing...", result.PresignedUrl);
    }

    // ---------------------------------------------------
    // 6) GetFileAsync
    // ---------------------------------------------------
    [Fact]
    public async Task GetFileAsync_WithSuccess_ReturnsFileData()
    {
        // Arrange
        var getFileDto = new GetFileInfoDto("my-bucket", "folder/video.mkv");
        var expectedFileData = new FileData(
            new MemoryStream([1, 2, 3]),
            "video/x-matroska",
            "video.mkv"
        );

        _bucketRepositoryMock
            .Setup(repo => repo.DownloadFileAsync("my-bucket", "folder/video.mkv"))
            .ReturnsAsync(expectedFileData);

        // Act
        var result = await _service.GetFileAsync(getFileDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("video/x-matroska", result.ContentType);
        Assert.Equal("video.mkv", result.FileName);
    }

    [Fact]
    public async Task GetFileAsync_WhenExceptionThrown_ThrowsInternalServerErrorException()
    {
        // Arrange
        var getFileDto = new GetFileInfoDto("bucketFail", "keyFail");
        _bucketRepositoryMock
            .Setup(repo => repo.DownloadFileAsync("bucketFail", "keyFail"))
            .Throws(new InvalidOperationException("Cannot download"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InternalServerErrorException>(() => _service.GetFileAsync(getFileDto));
        Assert.Contains("Failed to get file", ex.Message);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    // ---------------------------------------------------
    // 7) UpdateFile
    // ---------------------------------------------------
    [Fact]
    public void UpdateFile_EnqueuesTask_AndReturnsProcessingInfo()
    {
        // Arrange
        var file = new UploadedFileInfoDto("path/vid.mp4", "video/mp4", "vid.mp4");
        var dto = new UpdateFileInfoDto(
            file,
            "videoBucket",
            "episode2/vid.mp4",
            true,
            FileDestination.ContentImageUrl,
            "http://cdn2.com"
        );

        // Act
        var result = _service.UpdateFile(dto);

        // Assert
        _fileProcessingQueueMock.Verify(
            q => q.Enqueue(
                It.Is<FileProcessingTask>(
                    t =>
                        t.File == file &&
                        t.BucketName == "videoBucket" &&
                        t.Key == "episode2/vid.mp4" &&
                        t.IsVideoProcNecessary == true &&
                        t.Destination == FileDestination.ContentImageUrl &&
                        t.BaseUrl == "http://cdn2.com"
                )
            ),
            Times.Once
        );
        Assert.Equal("videoBucket", result.BucketName);
        Assert.Equal("Processing...", result.PresignedUrl);
    }

    // ---------------------------------------------------
    // 8) DeleteFileAsync
    // ---------------------------------------------------
    [Fact]
    public async Task DeleteFileAsync_WithSuccess_NoExceptions()
    {
        // Arrange
        var deleteDto = new DeleteFileInfoDto("any-bucket", "someKey");

        _bucketRepositoryMock
            .Setup(repo => repo.DeleteFileAsync("any-bucket", "someKey"))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteFileAsync(deleteDto);

        // Assert
        _bucketRepositoryMock.Verify(
            x => x.DeleteFileAsync("any-bucket", "someKey"),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteFileAsync_WhenExceptionThrown_ThrowsInternalServerErrorException()
    {
        // Arrange
        var deleteDto = new DeleteFileInfoDto("bad-bucket", "bad-key");
        _bucketRepositoryMock
            .Setup(repo => repo.DeleteFileAsync("bad-bucket", "bad-key"))
            .Throws(new InvalidOperationException("Cannot delete"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InternalServerErrorException>(() => _service.DeleteFileAsync(deleteDto));
        Assert.Contains("Failed to delete file", ex.Message);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }
}
