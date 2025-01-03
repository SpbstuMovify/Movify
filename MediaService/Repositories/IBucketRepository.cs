using Amazon.S3.Model;
using MediaService.Dtos.S3;
using MediaService.Models;

namespace MediaService.Repositories;

public interface IBucketRepository
{
    Task<IList<S3BucketDto>> GetBucketsAsync();
    Task<S3BucketDto> CreateBucketAsync(string bucketName);
    Task DeleteBucketAsync(string bucketName);
    Task<IList<S3ObjectDto>> GetFilesAsync(string bucketName, string prefix);
    Task<S3ObjectDto> UploadFileAsync(IFormFile file, string bucketName, string key);
    Task<DownloadedFile> DownloadFileAsync(string bucketName, string key);
    Task DeleteFileAsync(string bucketName, string key);
}