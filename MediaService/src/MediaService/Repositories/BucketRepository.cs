using System.Net;

using Amazon.S3;
using Amazon.S3.Model;

using MediaService.Dtos.S3;
using MediaService.Utils.Exceptions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace MediaService.Repositories;

public class BucketRepository(
    ILogger<BucketRepository> logger,
    IAmazonS3 s3Client
) : IBucketRepository
{
    public async Task<IList<S3BucketDto>> GetBucketsAsync()
    {
        var response = await s3Client.ListBucketsAsync();

        switch (response.HttpStatusCode)
        {
            case HttpStatusCode.OK: return response.Buckets.Select(b => new S3BucketDto(b.BucketName)).ToList();
            default:
                logger.LogWarning($"Unexpected status code from S3: {response.HttpStatusCode}");
                throw new AmazonS3Exception($"Unexpected status code from S3: {response.HttpStatusCode}");
        }
    }

    public async Task<S3BucketDto> CreateBucketAsync(string bucketName)
    {
        if (await DoesBucketExist(bucketName))
        {
            logger.LogWarning($"Bucket '{bucketName}' does not exist");
            throw new AmazonS3Exception($"Bucket {bucketName} already exists");
        }

        var response = await s3Client.PutBucketAsync(bucketName);

        switch (response.HttpStatusCode)
        {
            case HttpStatusCode.OK: return new S3BucketDto(bucketName);
            default:
                logger.LogWarning($"Unexpected status code from S3: {response.HttpStatusCode}");
                throw new AmazonS3Exception($"Unexpected status code from S3: {response.HttpStatusCode}");
        }
    }

    public async Task DeleteBucketAsync(string bucketName)
    {
        if (!await DoesBucketExist(bucketName))
        {
            logger.LogWarning($"Bucket '{bucketName}' does not exist");
            throw new NotFoundException($"Bucket {bucketName} does not exist");
        }

        var response = await s3Client.DeleteBucketAsync(bucketName);

        switch (response.HttpStatusCode)
        {
            case HttpStatusCode.NoContent: return;
            default:
                logger.LogWarning($"Unexpected status code from S3: {response.HttpStatusCode}");
                throw new AmazonS3Exception($"Unexpected status code from S3: {response.HttpStatusCode}");
        }
    }

    public async Task<IList<S3ObjectDto>> GetFilesAsync(
        string bucketName,
        string prefix
    )
    {
        if (!await DoesBucketExist(bucketName))
        {
            logger.LogWarning($"Bucket '{bucketName}' does not exist");
            throw new NotFoundException($"Bucket {bucketName} does not exist");
        }

        var request = new ListObjectsV2Request()
        {
            BucketName = bucketName,
            Prefix = prefix
        };

        var response = await s3Client.ListObjectsV2Async(request);

        switch (response.HttpStatusCode)
        {
            case HttpStatusCode.OK:
                return await Task.WhenAll(
                    response.S3Objects.Select(
                        async o => new S3ObjectDto(
                            o.BucketName,
                            await GetPreSignedUrlAsync(o.BucketName, o.Key)
                        )
                    ).ToList()
                );
            default:
                logger.LogWarning($"Unexpected status code from S3: {response.HttpStatusCode}");
                throw new AmazonS3Exception($"Unexpected status code from S3: {response.HttpStatusCode}");
        }
    }

    public async Task<S3ObjectDto> UploadFileAsync(
        FileData fileData,
        string bucketName,
        string key
    )
    {
        if (!await DoesBucketExist(bucketName))
        {
            logger.LogWarning($"Bucket '{bucketName}' does not exist");
            await CreateBucketAsync(bucketName);
        }

        var putRequest = new PutObjectRequest()
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileData.Content,
            ContentType = fileData.ContentType
        };

        var response = await s3Client.PutObjectAsync(putRequest);

        switch (response.HttpStatusCode)
        {
            case HttpStatusCode.OK:
                return new S3ObjectDto(
                    bucketName,
                    await GetPreSignedUrlAsync(bucketName, key)
                );
            default:
                logger.LogWarning($"Unexpected status code from S3: {response.HttpStatusCode}");
                throw new AmazonS3Exception($"Unexpected status code from S3: {response.HttpStatusCode}");
        }
    }

    public async Task<FileData> DownloadFileAsync(
        string bucketName,
        string key
    )
    {
        if (!await DoesBucketExist(bucketName))
        {
            logger.LogWarning($"Bucket '{bucketName}' does not exist");
            throw new NotFoundException($"Bucket {bucketName} does not exist");
        }

        if (!await DoesFileExist(bucketName, key))
        {
            logger.LogWarning($"File '{key}' in bucket '{bucketName}' does not exist");
            throw new NotFoundException($"File '{key}' in bucket '{bucketName}' does not exist");
        }

        var response = await s3Client.GetObjectAsync(bucketName, key);

        switch (response.HttpStatusCode)
        {
            case HttpStatusCode.OK:
                return new FileData(
                    response.ResponseStream,
                    response.Headers.ContentType,
                    key.Contains("/") ? key.Substring(key.LastIndexOf("/", StringComparison.Ordinal) + 1) : key
                );
            default:
                logger.LogWarning($"Unexpected status code from S3: {response.HttpStatusCode}");
                throw new AmazonS3Exception($"Unexpected status code from S3: {response.HttpStatusCode}");
        }
    }

    public async Task DeleteFileAsync(
        string bucketName,
        string key
    )
    {
        if (!await DoesBucketExist(bucketName))
        {
            logger.LogWarning($"Bucket '{bucketName}' does not exist");
            throw new NotFoundException($"Bucket {bucketName} does not exist");
        }

        if (!await DoesFileExist(bucketName, key))
        {
            logger.LogWarning($"File '{key}' in bucket '{bucketName}' does not exist");
            throw new NotFoundException($"File '{key}' in bucket '{bucketName}' does not exist");
        }

        var response = await s3Client.DeleteObjectAsync(bucketName, key);

        switch (response.HttpStatusCode)
        {
            case HttpStatusCode.NoContent: return;
            default:
                logger.LogWarning($"Unexpected status code from S3: {response.HttpStatusCode}");
                throw new AmazonS3Exception($"Unexpected status code from S3: {response.HttpStatusCode}");
        }
    }

    private async Task<string> GetPreSignedUrlAsync(
        string bucketName,
        string key
    )
    {
        var request = new GetPreSignedUrlRequest()
        {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(1)
        };

        return await s3Client.GetPreSignedURLAsync(request);
    }

    private async Task<bool> DoesBucketExist(string bucketName) { return await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName); }
    
    private async Task<bool> DoesFileExist(string bucketName, string key) 
    {
        try
        {
            var response = await s3Client.GetObjectMetadataAsync(bucketName, key);
            return true;
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
                return false;
            else
                throw;
        }
    }
}
