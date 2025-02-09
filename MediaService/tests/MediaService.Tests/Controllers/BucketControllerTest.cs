using FluentValidation;
using FluentValidation.Results;

using JetBrains.Annotations;

using MediaService.Controllers;
using MediaService.Controllers.Requests;
using MediaService.Dtos.Bucket;
using MediaService.Dtos.FileInfo;
using MediaService.FileProcessing;
using MediaService.Repositories;
using MediaService.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace MediaService.Tests.Controllers;

[TestSubject(typeof(BucketController))]
public class BucketControllerTest
{
    private readonly Mock<IBucketService> _serviceMock;

    private readonly BucketController _controller;

    public BucketControllerTest()
    {
        _serviceMock = new Mock<IBucketService>();

        _controller = new BucketController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_WithAdminRole_ReturnsOkResult()
    {
        // Arrange
        var buckets = new[]
        {
            new BucketDto("bucketA"),
            new BucketDto("bucketB")
        };

        _serviceMock
            .Setup(s => s.GetBucketsAsync())
            .ReturnsAsync(buckets);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        var value = Assert.IsAssignableFrom<BucketDto[]>(okResult.Value);
        Assert.Equal(2, value.Length);

        _serviceMock.Verify(s => s.GetBucketsAsync(), Times.Once);
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<CreateBucketRequest>>();
        validatorMock
            .Setup(
                v => v.ValidateAsync(
                    It.IsAny<ValidationContext<CreateBucketRequest>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new ValidationResult());

        var expectedDto = new BucketDto("created-bucket");
        _serviceMock
            .Setup(s => s.CreateBucketAsync(It.IsAny<CreateBucketDto>()))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.Create("created-bucket", validatorMock.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        var value = Assert.IsType<BucketDto>(okResult.Value);
        Assert.Equal("created-bucket", value.Name);

        validatorMock.Verify(
            v => v.ValidateAsync(
                It.Is<ValidationContext<CreateBucketRequest>>(r => r.InstanceToValidate.BucketName == "created-bucket"),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        _serviceMock.Verify(
            s => s.CreateBucketAsync(
                It.Is<CreateBucketDto>(d => d.Name == "created-bucket")
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Delete_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<DeleteBucketRequest>>();
        validatorMock
            .Setup(
                v => v.ValidateAsync(
                    It.IsAny<ValidationContext<DeleteBucketRequest>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.DeleteBucketAsync(It.IsAny<DeleteBucketDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete("bucket-to-delete", validatorMock.Object);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, noContentResult.StatusCode);

        validatorMock.Verify(
            v => v.ValidateAsync(
                It.Is<ValidationContext<DeleteBucketRequest>>(r => r.InstanceToValidate.BucketName == "bucket-to-delete"),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        _serviceMock.Verify(
            s => s.DeleteBucketAsync(
                It.Is<DeleteBucketDto>(d => d.Name == "bucket-to-delete")
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllFiles_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<GetFilesRequest>>();
        validatorMock
            .Setup(
                v => v.ValidateAsync(
                    It.IsAny<ValidationContext<GetFilesRequest>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new ValidationResult());

        var files = new[]
        {
            new FileInfoDto("bucketX", "http://url1"),
            new FileInfoDto("bucketX", "http://url2")
        };

        _serviceMock
            .Setup(s => s.GetFilesAsync(It.IsAny<GetFilesInfoDto>()))
            .ReturnsAsync(files);

        // Act
        var result = await _controller.GetAllFiles("bucketX", "test", validatorMock.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        var value = Assert.IsAssignableFrom<FileInfoDto[]>(okResult.Value);
        Assert.Equal(2, value.Length);

        validatorMock.Verify(
            v => v.ValidateAsync(
                It.Is<ValidationContext<GetFilesRequest>>(
                    r
                        => r.InstanceToValidate.BucketName == "bucketX" &&
                           r.InstanceToValidate.Prefix == "test"
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        _serviceMock.Verify(
            s => s.GetFilesAsync(
                It.Is<GetFilesInfoDto>(d => d.BucketName == "bucketX" && d.Prefix == "test")
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateFile_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<CreateFileRequest>>();
        validatorMock
            .Setup(
                v => v.ValidateAsync(
                    It.IsAny<ValidationContext<CreateFileRequest>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new ValidationResult());

        // Создаём фейковый IFormFile
        const string testFileName = "myfile.txt";
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(testFileName);
        fileMock.Setup(f => f.ContentType).Returns("text/plain");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

        var returnedDto = new FileInfoDto("bucket-file", "Processing...");
        _serviceMock
            .Setup(s => s.CreateFile(It.IsAny<CreateFileInfoDto>()))
            .Returns(returnedDto);

        if (!Directory.Exists(".tmp"))
        {
            Directory.CreateDirectory(".tmp");
        }

        var beforeSubdirs = Directory.GetDirectories(".tmp");

        // Мы должны подделать Request, чтобы в controller.CreateFile(...) 
        // поле Request.Path.Value содержало нужную строку
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/v1/buckets/bucket-file/files";
        _controller.ControllerContext.HttpContext = httpContext;

        // Act
        var result = await _controller.CreateFile(
            file: fileMock.Object,
            bucketName: "bucket-file",
            prefix: "docs",
            isVideoProcNecessary: true,
            destination: FileDestination.ContentImageUrl,
            validator: validatorMock.Object
        );

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        var value = Assert.IsType<FileInfoDto>(okResult.Value);
        Assert.Equal("bucket-file", value.BucketName);
        Assert.Equal("Processing...", value.PresignedUrl);

        var afterSubdirs = Directory.GetDirectories(".tmp");
        var newFolders = afterSubdirs.Except(beforeSubdirs).ToArray();

        Assert.Single(newFolders);

        var createdSubdir = newFolders[0];
        var filePath = Path.Combine(createdSubdir, testFileName);

        Assert.True(File.Exists(filePath), $"Ожидали файл {filePath}, но его нет.");

        // 3. Удаляем (cleanup)
        Directory.Delete(createdSubdir, recursive: true);

        validatorMock.Verify(
            v => v.ValidateAsync(
                It.Is<ValidationContext<CreateFileRequest>>(
                    r =>
                        r.InstanceToValidate.BucketName == "bucket-file" &&
                        r.InstanceToValidate.Prefix == "docs" &&
                        r.InstanceToValidate.IsVideoProcNecessary == true &&
                        r.InstanceToValidate.Destination == FileDestination.ContentImageUrl
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        _serviceMock.Verify(
            s => s.CreateFile(It.IsAny<CreateFileInfoDto>()),
            Times.Once
        );
    }

    [Fact]
    public async Task GetFile_WithValidRequest_ReturnsFile()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<GetFileRequest>>();
        validatorMock
            .Setup(
                v => v.ValidateAsync(
                    It.IsAny<ValidationContext<GetFileRequest>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new ValidationResult());

        var fileData = new FileData(
            new MemoryStream([1, 2, 3]),
            "image/png",
            "test.png"
        );

        _serviceMock
            .Setup(s => s.GetFileAsync(It.IsAny<GetFileInfoDto>()))
            .ReturnsAsync(fileData);

        // Act
        var result = await _controller.GetFile("bucketX", "some/key", validatorMock.Object);

        // Assert
        var fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal("image/png", fileResult.ContentType);
        Assert.Equal("test.png", fileResult.FileDownloadName);

        validatorMock.Verify(
            v => v.ValidateAsync(
                It.Is<ValidationContext<GetFileRequest>>(
                    r =>
                        r.InstanceToValidate.BucketName == "bucketX" &&
                        r.InstanceToValidate.Key == "some/key"
                ),
                CancellationToken.None
            ),
            Times.Once
        );

        _serviceMock.Verify(
            s => s.GetFileAsync(It.Is<GetFileInfoDto>(d => d.BucketName == "bucketX" && d.Key == "some/key")),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateFile_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<UpdateFileRequest>>();
        validatorMock
            .Setup(
                v => v.ValidateAsync(
                    It.IsAny<ValidationContext<UpdateFileRequest>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new ValidationResult());

        const string testFileName = "update.txt";
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(testFileName);
        fileMock.Setup(f => f.ContentType).Returns("video/mp4");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

        var returnedDto = new FileInfoDto("bucketUpdate", "Processing...");
        _serviceMock
            .Setup(s => s.UpdateFile(It.IsAny<UpdateFileInfoDto>()))
            .Returns(returnedDto);

        if (!Directory.Exists(".tmp"))
        {
            Directory.CreateDirectory(".tmp");
        }

        var beforeSubdirs = Directory.GetDirectories(".tmp");

        var httpContext = new DefaultHttpContext { Request = { Path = "/v1/buckets/bucketUpdate/files/myVid" } };
        _controller.ControllerContext.HttpContext = httpContext;

        // Act
        var result = await _controller.UpdateFile(
            file: fileMock.Object,
            bucketName: "bucketUpdate",
            key: "myVid",
            isVideoProcNecessary: false,
            destination: FileDestination.Internal,
            validator: validatorMock.Object
        );

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<FileInfoDto>(okResult.Value);
        Assert.Equal("bucketUpdate", value.BucketName);
        Assert.Equal("Processing...", value.PresignedUrl);

        var afterSubdirs = Directory.GetDirectories(".tmp");
        var newFolders = afterSubdirs.Except(beforeSubdirs).ToArray();

        Assert.Single(newFolders);

        var createdSubdir = newFolders[0];
        var filePath = Path.Combine(createdSubdir, testFileName);

        Assert.True(File.Exists(filePath), $"Ожидали файл {filePath}, но его нет.");

        // 3. Удаляем (cleanup)
        Directory.Delete(createdSubdir, recursive: true);

        validatorMock.Verify(
            v => v.ValidateAsync(
                It.Is<ValidationContext<UpdateFileRequest>>(
                    r =>
                        r.InstanceToValidate.BucketName == "bucketUpdate" &&
                        r.InstanceToValidate.Key == "myVid" &&
                        r.InstanceToValidate.IsVideoProcNecessary == false &&
                        r.InstanceToValidate.Destination == FileDestination.Internal
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        _serviceMock.Verify(
            s => s.UpdateFile(It.IsAny<UpdateFileInfoDto>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteFile_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<DeleteFileRequest>>();
        validatorMock
            .Setup(
                v => v.ValidateAsync(
                    It.IsAny<ValidationContext<DeleteFileRequest>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new ValidationResult());

        _serviceMock
            .Setup(s => s.DeleteFileAsync(It.IsAny<DeleteFileInfoDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteFile("bucketZ", "folder/file.txt", validatorMock.Object);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, noContentResult.StatusCode);

        validatorMock.Verify(
            v => v.ValidateAsync(
                It.Is<ValidationContext<DeleteFileRequest>>(
                    r =>
                        r.InstanceToValidate.BucketName == "bucketZ" &&
                        r.InstanceToValidate.Key == "folder/file.txt"
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        _serviceMock.Verify(
            s => s.DeleteFileAsync(
                It.Is<DeleteFileInfoDto>(
                    d =>
                        d.BucketName == "bucketZ" && d.Key == "folder/file.txt"
                )
            ),
            Times.Once
        );
    }
}
