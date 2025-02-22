using ChunkerService.Dtos.S3;

namespace ChunkerService.Repositories;

public interface IChunkerRepository
{
    Task<S3ObjectDto> UploadFileAsync(
        FileData fileData,
        string bucketName,
        string key
    );

    Task<FileData> DownloadFileAsync(
        string bucketName,
        string key
    );
}
