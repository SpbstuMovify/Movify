using System.Net;
using Amazon.S3;
using Amazon.S3.Model;

using ChunkerService.Dtos;
using ChunkerService.Dtos.S3;
using ChunkerService.Utils.Exceptions;

namespace ChunkerService.Repositories;

public class ChunkerRepository(ILogger<ChunkerRepository> logger, IAmazonS3 s3Client) : IChunkerRepository
{
    public async Task<S3ObjectDto> UploadFileAsync(UploadedFile file, string bucketName, string key)
    {
        if (!await DoesBucketExist(bucketName))
        {
            logger.LogWarning($"Bucket '{bucketName}' does not exist");
            throw new ResourceNotFoundException($"Bucket {bucketName} does not exist.");
        }

        var putRequest = new PutObjectRequest()
        {
            BucketName = bucketName,
            Key = key,
            InputStream = file.Content,
            ContentType = file.ContentType
        };

        var response = await s3Client.PutObjectAsync(putRequest);

        switch (response.HttpStatusCode)
        {
            case HttpStatusCode.OK:
                return  new S3ObjectDto
                {
                    BucketName = bucketName,
                    PresignedUrl = await GetPreSignedURLAsync(bucketName, key)
                };
            default:
                logger.LogWarning($"Unexpected status code from S3: {response.HttpStatusCode}");
                throw new AmazonS3Exception($"Unexpected status code from S3: {response.HttpStatusCode}");
        }
    }

    public async Task<DownloadedFile> DownloadFileAsync(string bucketName, string key)
    {
        if (!await DoesBucketExist(bucketName))
        {
            logger.LogWarning($"Bucket '{bucketName}' does not exist");
            throw new ResourceNotFoundException($"Bucket {bucketName} does not exist.");
        }

        var response = await s3Client.GetObjectAsync(bucketName, key);

        switch (response.HttpStatusCode)
        {
            case HttpStatusCode.OK:
                return new DownloadedFile
                {
                    Content = response.ResponseStream,
                    ContentType = response.Headers.ContentType,
                    FileName = key.Contains("/") ? key.Substring(key.LastIndexOf("/") + 1) : key
                };
            case HttpStatusCode.NotFound:
                logger.LogWarning($"File in bucket[{bucketName}] by key[{key}] does not exist");
                throw new ResourceNotFoundException($"File in bucket[{bucketName}] by key[{key}] does not exist");
            default:
                logger.LogWarning($"Unexpected status code from S3: {response.HttpStatusCode}");
                throw new AmazonS3Exception($"Unexpected status code from S3: {response.HttpStatusCode}");
        }
    }

    public async Task DeleteFileAsync(string bucketName, string key)
    {
        if (!await DoesBucketExist(bucketName))
        {
            logger.LogWarning($"Bucket '{bucketName}' does not exist");
            throw new ResourceNotFoundException($"Bucket {bucketName} does not exist.");
        }

        var response = await s3Client.DeleteObjectAsync(bucketName, key);

        switch (response.HttpStatusCode)
        {
            case HttpStatusCode.OK:
                return;
            default:
                logger.LogWarning($"Unexpected status code from S3: {response.HttpStatusCode}");
                throw new AmazonS3Exception($"Unexpected status code from S3: {response.HttpStatusCode}");
        }
    }

    private async Task<string> GetPreSignedURLAsync(string bucketName, string key)
    {
        var request = new GetPreSignedUrlRequest()
        {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(1)
        };

        return await s3Client.GetPreSignedURLAsync(request);
    }

    private async Task<bool> DoesBucketExist(string bucketName)
    {
        return await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName);
    }
}
