using System.Net;

using Amazon.S3;
using Amazon.S3.Model;

using ChunkerService.Repositories;
using ChunkerService.Utils.Exceptions;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Moq;

namespace ChunkerService.Tests.Repositories;

[TestSubject(typeof(ChunkerRepository))]
public class ChunkerRepositoryTest
{
    private readonly Mock<IAmazonS3> _s3ClientMock;
    private readonly ChunkerRepository _chunkerRepository;

    public ChunkerRepositoryTest()
    {
        _s3ClientMock = new Mock<IAmazonS3>();

        var loggerMock = new Mock<ILogger<ChunkerRepository>>();

        _chunkerRepository = new ChunkerRepository(loggerMock.Object, _s3ClientMock.Object);
    }
    
    [Fact]
    public async Task UploadFileAsync_WithNonOkStatusCode_ReturnsS3ObjectDto()
    {
        // Arrange
        const string bucketName = "new-bucket";
        const string key = "test.txt";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetACLResponse { HttpStatusCode = HttpStatusCode.OK });

        _s3ClientMock
            .Setup(c => c.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new PutObjectResponse
                {
                    HttpStatusCode = HttpStatusCode.OK
                }
            );

        _s3ClientMock
            .Setup(c => c.GetPreSignedURLAsync(It.IsAny<GetPreSignedUrlRequest>()))
            .ReturnsAsync("http://upload-url");

        var fileData = new FileData(new MemoryStream(), "text/plain", "test.txt");

        // Act
        var result = await _chunkerRepository.UploadFileAsync(fileData, bucketName, key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bucketName, result.BucketName);
        Assert.Equal("http://upload-url", result.PresignedUrl);
    }

    [Fact]
    public async Task UploadFileAsync_WithNonExistingBucket_ThrowsResourceNotFoundException()
    {
        // Arrange
        const string bucketName = "new-bucket";
        const string key = "test.txt";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("NoSuchBucket") { ErrorCode = "NoSuchBucket" });

        var fileData = new FileData(new MemoryStream(), "text/plain", "test.txt");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _chunkerRepository.UploadFileAsync(fileData, bucketName, key));
    }

    [Fact]
    public async Task UploadFileAsync_WithNonOkStatusCode_ThrowsAmazonS3Exception()
    {
        // Arrange
        const string bucketName = "my-bucket";
        const string key = "fail.txt";
        var fileData = new FileData(new MemoryStream(), "text/plain", "fail.txt");

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetACLResponse { HttpStatusCode = HttpStatusCode.OK });

        _s3ClientMock
            .Setup(c => c.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new PutObjectResponse
                {
                    HttpStatusCode = HttpStatusCode.Conflict
                }
            );

        // Act & Assert
        await Assert.ThrowsAsync<AmazonS3Exception>(() => _chunkerRepository.UploadFileAsync(fileData, bucketName, key));
    }

    [Fact]
    public async Task DownloadFileAsync_WithHttpStatusCodeOK_ReturnsFileData()
    {
        // Arrange
        const string bucketName = "existing-bucket";
        const string key = "folder/test.txt";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetACLResponse { HttpStatusCode = HttpStatusCode.OK });

        var memoryStream = new MemoryStream();
        const string contentType = "text/plain";

        _s3ClientMock
            .Setup(c => c.GetObjectAsync(bucketName, key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new GetObjectResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    ResponseStream = memoryStream,
                    Headers = { ContentType = contentType }
                }
            );

        // Act
        var fileData = await _chunkerRepository.DownloadFileAsync(bucketName, key);

        // Assert
        Assert.NotNull(fileData);
        Assert.Equal(memoryStream, fileData.Content);
        Assert.Equal(contentType, fileData.ContentType);
        Assert.Equal("test.txt", fileData.FileName);
    }

    [Fact]
    public async Task DownloadFileAsync_WithNonExistingBucket_ThrowsResourceNotFoundException()
    {
        // Arrange
        const string bucketName = "no-bucket";
        const string key = "some.txt";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("NoSuchBucket") { ErrorCode = "NoSuchBucket" });

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _chunkerRepository.DownloadFileAsync(bucketName, key));
    }

    [Fact]
    public async Task DownloadFileAsync_WithHttpStatusCodeNotFound_ThrowsResourceNotFoundException()
    {
        // Arrange
        const string bucketName = "existing-bucket";
        const string key = "no-file.txt";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetACLResponse { HttpStatusCode = HttpStatusCode.OK });

        _s3ClientMock
            .Setup(c => c.GetObjectAsync(bucketName, key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new GetObjectResponse
                {
                    HttpStatusCode = HttpStatusCode.NotFound
                }
            );

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _chunkerRepository.DownloadFileAsync(bucketName, key));
    }
}
