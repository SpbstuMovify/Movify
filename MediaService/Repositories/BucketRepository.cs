using Amazon.S3;
using Amazon.S3.Model;

using MediaService.Dtos;
using MediaService.Models;

namespace MediaService.Repositories;

public class BucketRepository(ILogger<BucketRepository> logger, IAmazonS3 s3Client) : IBucketRepository
{
    public async Task<IList<S3Bucket>> GetBucketsAsync()
    {
        logger.LogDebug("Start fetching S3 buckets");
        var data = await s3Client.ListBucketsAsync();
        logger.LogDebug($"Successfully fetched {data.Buckets.Count} buckets");
        return data.Buckets;
    }

    public async Task<S3Bucket> CreateBucketAsync(string bucketName)
    {
        logger.LogDebug($"Checking if bucket '{bucketName}' exists");
        await CheckBucketExits(bucketName);

        logger.LogDebug($"Creating bucket '{bucketName}'");
        await s3Client.PutBucketAsync(bucketName);

        logger.LogDebug($"Fetching created bucket '{bucketName}'");
        var response = await s3Client.ListBucketsAsync();
        var bucket = response.Buckets.FirstOrDefault(b => b.BucketName == bucketName);

        if (bucket == null)
        {
            logger.LogError($"Bucket '{bucketName}' was not found after creation");
            throw new InvalidOperationException($"Bucket {bucketName} does not exist");
        }

        logger.LogDebug($"Successfully created and fetched bucket '{bucketName}'");
        return bucket;
    }

    public async Task DeleteBucketAsync(string bucketName)
    {
        logger.LogDebug($"Deleting bucket '{bucketName}'");
        await s3Client.DeleteBucketAsync(bucketName);
        logger.LogDebug($"Successfully deleted bucket '{bucketName}'");
    }

    public async Task<IList<S3Object>> GetFilesAsync(string bucketName, string prefix)
    {
        logger.LogDebug($"Checking if bucket '{bucketName}' exists");
        await CheckBucketExits(bucketName);

        logger.LogDebug($"Fetching files from bucket '{bucketName}' with prefix '{prefix}'");
        var request = new ListObjectsV2Request()
        {
            BucketName = bucketName,
            Prefix = prefix
        };

        var result = await s3Client.ListObjectsV2Async(request);
        logger.LogDebug($"Successfully fetched {result.S3Objects.Count} files from bucket '{bucketName}' with prefix '{prefix}'");
        return result.S3Objects;
    }

    public async Task<S3Object> UploadFileAsync(IFormFile file, string bucketName, string key)
    {
        if (file.Length == 0)
        {
            logger.LogWarning("File is empty and cannot be uploaded");
            throw new ArgumentException("File is required and cannot be empty.", nameof(file));
        }

        logger.LogDebug($"Checking if bucket '{bucketName}' exists");
        await CheckBucketExits(bucketName);

        logger.LogDebug($"Uploading file to bucket '{bucketName}' with key '{key}'");
        var putRequest = new PutObjectRequest()
        {
            BucketName = bucketName,
            Key = key,
            InputStream = file.OpenReadStream(),
            ContentType = file.ContentType
        };

        await s3Client.PutObjectAsync(putRequest);

        logger.LogDebug($"Verifying uploaded file in bucket '{bucketName}' with key '{key}'");
        var listRequest = new ListObjectsV2Request()
        {
            BucketName = bucketName,
            MaxKeys = 1,
            StartAfter = key
        };

        var result = await s3Client.ListObjectsV2Async(listRequest);
        if (result.S3Objects.Count == 0)
        {
            logger.LogError($"Uploaded file with key '{key}' in bucket '{bucketName}' not found");
            throw new InvalidOperationException($"File with key '{key}' in bucket '{bucketName}' does not exist.");
        }

        logger.LogDebug($"Successfully uploaded file to bucket '{bucketName}' with key '{key}' successful");
        return result.S3Objects[0];
    }

    public async Task<DownloadedFile> DownloadFileAsync(string bucketName, string key)
    {
        logger.LogDebug($"Checking if bucket '{bucketName}' exists");
        await CheckBucketExits(bucketName);

        logger.LogDebug($"Downloading file with key '{key}' from bucket '{bucketName}'");
        var s3Object = await s3Client.GetObjectAsync(bucketName, key);

        logger.LogDebug($"Successfully downloaded file with key '{key}' from bucket '{bucketName}'");
        return new DownloadedFile
        {
            Content = s3Object.ResponseStream,
            ContentType = s3Object.Headers.ContentType,
            FileName = key.Contains("/") ? key.Substring(key.LastIndexOf("/") + 1) : key
        };
    }

    public async Task DeleteFileAsync(string bucketName, string key)
    {
        logger.LogInformation($"Checking if bucket '{bucketName}' exists");
        await CheckBucketExits(bucketName);

        logger.LogInformation($"Deleting file with key '{key}' from bucket '{bucketName}'");
        await s3Client.DeleteObjectAsync(bucketName, key);
        logger.LogInformation($"Successfully deleted file with key '{key}' from bucket '{bucketName}'");
    }

    private async Task CheckBucketExits(string bucketName)
    {
        var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName);
        if (!bucketExists)
        {
            logger.LogWarning($"Bucket '{bucketName}' does not exist");
            throw new InvalidOperationException($"Bucket {bucketName} does not exist.");
        }
    }
}