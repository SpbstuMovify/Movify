using Amazon.S3.Model;
using ChunkerService.Dtos.S3;

namespace ChunkerService.Repositories;

public interface IChunkerRepository
{
    Task<S3ObjectDto> UploadFileAsync(UploadedFile file, string bucketName, string key);
    Task<DownloadedFile> DownloadFileAsync(string bucketName, string key);
    Task DeleteFileAsync(string bucketName, string key);
}
