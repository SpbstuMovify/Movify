using Amazon.S3.Model;
using MediaService.Models;

namespace MediaService.Repositories;

public interface IBucketRepository
{
    Task<IList<S3Bucket>> GetBucketsAsync();
    Task<S3Bucket> CreateBucketAsync(string bucketName);
    Task DeleteBucketAsync(string bucketName);
    Task<IList<S3Object>> GetFilesAsync(string bucketName, string prefix);
    Task<S3Object> UploadFileAsync(IFormFile file, string bucketName, string prefix);
    Task<DownloadedFile> DownloadFileAsync(string bucketName, string key);
    Task DeleteFileAsync(string bucketName, string key);
}