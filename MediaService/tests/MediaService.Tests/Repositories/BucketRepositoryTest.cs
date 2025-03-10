using System.Net;

using Amazon.S3;
using Amazon.S3.Model;

using JetBrains.Annotations;

using MediaService.Dtos.S3;
using MediaService.Repositories;
using MediaService.Utils.Exceptions;

using Microsoft.Extensions.Logging;

using Moq;

namespace MediaService.Tests.Repositories;

[TestSubject(typeof(BucketRepository))]
public class BucketRepositoryTest
{
    private readonly Mock<IAmazonS3> _s3ClientMock;
    private readonly BucketRepository _bucketRepository;

    public BucketRepositoryTest()
    {
        _s3ClientMock = new Mock<IAmazonS3>();

        var loggerMock = new Mock<ILogger<BucketRepository>>();

        _bucketRepository = new BucketRepository(loggerMock.Object, _s3ClientMock.Object);
    }

    #region GetBucketsAsync

    [Fact]
    public async Task GetBucketsAsync_WithHttpStatusCodeOK_ReturnsListOfBuckets()
    {
        // Arrange
        var buckets = new List<S3Bucket>
        {
            new() { BucketName = "test-bucket-1" },
            new() { BucketName = "test-bucket-2" }
        };

        _s3ClientMock
            .Setup(c => c.ListBucketsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ListBucketsResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Buckets = buckets
                }
            );

        // Act
        var result = await _bucketRepository.GetBucketsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("test-bucket-1", result[0].BucketName);
        Assert.Equal("test-bucket-2", result[1].BucketName);
    }

    [Fact]
    public async Task GetBucketsAsync_WithUnexpectedStatusCode_ThrowsAmazonS3Exception()
    {
        // Arrange
        _s3ClientMock
            .Setup(c => c.ListBucketsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ListBucketsResponse
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError
                }
            );

        // Act & Assert
        await Assert.ThrowsAsync<AmazonS3Exception>(() => _bucketRepository.GetBucketsAsync());
    }

    #endregion

    #region CreateBucketAsync

    [Fact]
    public async Task CreateBucketAsync_WithExistingBucket_ThrowsAmazonS3Exception()
    {
        // Arrange
        var bucketName = "existing-bucket";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new GetACLResponse
                {
                    HttpStatusCode = HttpStatusCode.OK
                }
            );

        _s3ClientMock
            .Setup(c => c.PutBucketAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception("Should not be called"));

        // Act & Assert
        await Assert.ThrowsAsync<AmazonS3Exception>(() => _bucketRepository.CreateBucketAsync(bucketName));
    }

    [Fact]
    public async Task CreateBucketAsync_WithValidData_ReturnsBucketDto()
    {
        // Arrange
        var bucketName = "new-bucket";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new AmazonS3Exception("NoSuchBucket")
                {
                    ErrorCode = "NoSuchBucket"
                }
            );

        _s3ClientMock
            .Setup(c => c.PutBucketAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new PutBucketResponse
                {
                    HttpStatusCode = HttpStatusCode.OK
                }
            );

        // Act
        var result = await _bucketRepository.CreateBucketAsync(bucketName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bucketName, result.BucketName);
    }

    [Fact]
    public async Task CreateBucketAsync_WithNonOkStatusCode_ThrowsAmazonS3Exception()
    {
        // Arrange
        var bucketName = "fail-bucket";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("NoSuchBucket") { ErrorCode = "NoSuchBucket" });

        _s3ClientMock
            .Setup(c => c.PutBucketAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new PutBucketResponse
                {
                    HttpStatusCode = HttpStatusCode.BadRequest
                }
            );

        // Act & Assert
        await Assert.ThrowsAsync<AmazonS3Exception>(() => _bucketRepository.CreateBucketAsync(bucketName));
    }

    #endregion

    #region DeleteBucketAsync

    [Fact]
    public async Task DeleteBucketAsync_WithHttpStatusCodeNoContent_DoesNotThrow()
    {
        // Arrange
        var bucketName = "some-bucket";

        _s3ClientMock
            .Setup(c => c.ListBucketsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ListBucketsResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                Buckets = new List<S3Bucket> { new S3Bucket { BucketName = bucketName } }
            });

        _s3ClientMock
            .Setup(c => c.DeleteBucketAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteBucketResponse { HttpStatusCode = HttpStatusCode.NoContent });

        // Act
        var exception = await Record.ExceptionAsync(() => _bucketRepository.DeleteBucketAsync(bucketName));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task DeleteBucketAsync_WithUnexpectedStatusCode_ThrowsAmazonS3Exception()
    {
        // Arrange
        var bucketName = "some-bucket";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetACLResponse { HttpStatusCode = HttpStatusCode.OK });

        _s3ClientMock
            .Setup(c => c.DeleteBucketAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new DeleteBucketResponse
                {
                    HttpStatusCode = HttpStatusCode.Forbidden
                }
            );

        // Act & Assert
        await Assert.ThrowsAsync<AmazonS3Exception>(() => _bucketRepository.DeleteBucketAsync(bucketName));
    }

    #endregion

    #region GetFilesAsync

    [Fact]
    public async Task GetFilesAsync_WithNonExistingBucket_ThrowsResourceNotFoundException()
    {
        // Arrange
        var bucketName = "non-existing-bucket";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new AmazonS3Exception("NoSuchBucket")
                {
                    ErrorCode = "NoSuchBucket"
                }
            );

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _bucketRepository.GetFilesAsync(bucketName, "some-prefix"));
    }

    [Fact]
    public async Task GetFilesAsync_WithHttpStatusCodeOK_ReturnsListOfS3Objects()
    {
        // Arrange
        var bucketName = "my-bucket";
        var prefix = "folder/";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetACLResponse { HttpStatusCode = HttpStatusCode.OK });

        _s3ClientMock
            .Setup(c => c.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ListObjectsV2Response
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    S3Objects =
                    [
                        new() { BucketName = bucketName, Key = "folder/file1.txt" },
                        new() { BucketName = bucketName, Key = "folder/file2.txt" }
                    ]
                }
            );

        _s3ClientMock
            .Setup(c => c.GetPreSignedURLAsync(It.IsAny<GetPreSignedUrlRequest>()))
            .ReturnsAsync("http://test-url");

        // Act
        var result = await _bucketRepository.GetFilesAsync(bucketName, prefix);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, obj => { Assert.Equal(new S3ObjectDto(bucketName, "http://test-url"), obj); });
    }

    [Fact]
    public async Task GetFilesAsync_WithNonOkStatusCode_ThrowsAmazonS3Exception()
    {
        // Arrange
        var bucketName = "my-bucket";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetACLResponse { HttpStatusCode = HttpStatusCode.OK });

        _s3ClientMock
            .Setup(c => c.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ListObjectsV2Response
                {
                    HttpStatusCode = HttpStatusCode.ServiceUnavailable
                }
            );

        // Act & Assert
        await Assert.ThrowsAsync<AmazonS3Exception>(() => _bucketRepository.GetFilesAsync(bucketName, ""));
    }

    #endregion

    #region UploadFileAsync

    [Fact]
    public async Task UploadFileAsync_WithNonExistingBucket_CreatesBucket()
    {
        // Arrange
        var bucketName = "new-bucket";
        var key = "test.txt";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("NoSuchBucket") { ErrorCode = "NoSuchBucket" });

        _s3ClientMock
            .Setup(c => c.PutBucketAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutBucketResponse { HttpStatusCode = HttpStatusCode.OK });

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
        var result = await _bucketRepository.UploadFileAsync(fileData, bucketName, key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bucketName, result.BucketName);
        Assert.Equal("http://upload-url", result.PresignedUrl);
    }

    [Fact]
    public async Task UploadFileAsync_WithNonOkStatusCode_ThrowsAmazonS3Exception()
    {
        // Arrange
        var bucketName = "my-bucket";
        var key = "fail.txt";
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
        await Assert.ThrowsAsync<AmazonS3Exception>(() => _bucketRepository.UploadFileAsync(fileData, bucketName, key));
    }

    #endregion

    #region DownloadFileAsync

    [Fact]
    public async Task DownloadFileAsync_WithNonExistingBucket_ThrowsResourceNotFoundException()
    {
        // Arrange
        var bucketName = "no-bucket";
        var key = "some.txt";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("NoSuchBucket") { ErrorCode = "NoSuchBucket" });

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _bucketRepository.DownloadFileAsync(bucketName, key));
    }

    [Fact]
    public async Task DownloadFileAsync_WithFileNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var bucketName = "existing-bucket";
        var key = "no-file.txt";

        _s3ClientMock
            .Setup(c => c.ListBucketsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ListBucketsResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                Buckets = new List<S3Bucket> { new S3Bucket { BucketName = bucketName } }
            });

        _s3ClientMock
            .Setup(c => c.GetObjectMetadataAsync(bucketName, key, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Not found") { StatusCode = HttpStatusCode.NotFound });

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _bucketRepository.DownloadFileAsync(bucketName, key));
    }

    [Fact]
    public async Task DownloadFileAsync_WithHttpStatusCodeOK_ReturnsFileData()
    {
        // Arrange
        var bucketName = "existing-bucket";
        var key = "folder/test.txt";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetACLResponse { HttpStatusCode = HttpStatusCode.OK });

        var memoryStream = new MemoryStream();
        var contentType = "text/plain";

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
        var fileData = await _bucketRepository.DownloadFileAsync(bucketName, key);

        // Assert
        Assert.NotNull(fileData);
        Assert.Equal(memoryStream, fileData.Content);
        Assert.Equal(contentType, fileData.ContentType);
        Assert.Equal("test.txt", fileData.FileName);
    }

    #endregion

    #region DeleteFileAsync

    [Fact]
    public async Task DeleteFileAsync_WithNonExistingBucket_ThrowsResourceNotFoundException()
    {
        // Arrange
        var bucketName = "no-bucket";
        var key = "file.txt";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("NoSuchBucket") { ErrorCode = "NoSuchBucket" });

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _bucketRepository.DeleteFileAsync(bucketName, key)
        );
    }

    [Fact]
    public async Task DeleteFileAsync_WithHttpStatusCodeNoContent_DoesNotThrow()
    {
        // Arrange
        var bucketName = "existing-bucket";
        var key = "file.txt";

        _s3ClientMock
            .Setup(c => c.ListBucketsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ListBucketsResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                Buckets = new List<S3Bucket> { new S3Bucket { BucketName = bucketName } }
            });

        _s3ClientMock
            .Setup(c => c.GetObjectMetadataAsync(bucketName, key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetObjectMetadataResponse { HttpStatusCode = HttpStatusCode.OK });

        _s3ClientMock
            .Setup(c => c.DeleteObjectAsync(bucketName, key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectResponse { HttpStatusCode = HttpStatusCode.NoContent });

        // Act
        var exception = await Record.ExceptionAsync(() => _bucketRepository.DeleteFileAsync(bucketName, key));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task DeleteFileAsync_WithNonOkStatusCode_ThrowsAmazonS3Exception()
    {
        // Arrange
        var bucketName = "existing-bucket";
        var key = "file.txt";

        _s3ClientMock
            .Setup(c => c.GetACLAsync(bucketName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetACLResponse { HttpStatusCode = HttpStatusCode.OK });

        _s3ClientMock
            .Setup(c => c.DeleteObjectAsync(bucketName, key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new DeleteObjectResponse
                {
                    HttpStatusCode = HttpStatusCode.Unauthorized
                }
            );

        // Act & Assert
        await Assert.ThrowsAsync<AmazonS3Exception>(() => _bucketRepository.DeleteFileAsync(bucketName, key));
    }

    #endregion
}
