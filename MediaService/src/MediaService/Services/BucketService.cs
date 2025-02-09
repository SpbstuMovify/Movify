using MediaService.Dtos.Bucket;
using MediaService.Dtos.FileInfo;
using MediaService.FileProcessing;
using MediaService.Repositories;
using MediaService.Utils.Exceptions;

namespace MediaService.Services;

public class BucketService(
    IBucketRepository bucketRepository,
    IFileProcessingQueue fileProcessingQueue
) : IBucketService
{
    public async Task<IList<BucketDto>> GetBucketsAsync()
    {
        try
        {
            var data = await bucketRepository.GetBucketsAsync();

            return data.Select(b => new BucketDto(b.BucketName)).ToList();
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new InternalServerErrorException("Failed to get buckets", ex);
        }
    }

    public async Task<BucketDto> CreateBucketAsync(CreateBucketDto dto)
    {
        try
        {
            var data = await bucketRepository.CreateBucketAsync(dto.Name);

            return new BucketDto(data.BucketName);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new InternalServerErrorException("Failed to create bucket", ex);
        }
    }

    public async Task DeleteBucketAsync(DeleteBucketDto dto)
    {
        try
        {
            await bucketRepository.DeleteBucketAsync(dto.Name);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new InternalServerErrorException("Failed to delete bucket", ex);
        }
    }

    public async Task<IList<FileInfoDto>> GetFilesAsync(GetFilesInfoDto dto)
    {
        try
        {
            var data = await bucketRepository.GetFilesAsync(dto.BucketName, dto.Prefix);

            return data.Select(f => new FileInfoDto(f.BucketName, f.PresignedUrl)).ToList();
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new InternalServerErrorException("Failed to get files", ex);
        }
    }

    public FileInfoDto CreateFile(CreateFileInfoDto dto)
    {
        var fileName = dto.File.FileName;
        var bucketName = dto.BucketName;
        var prefix = dto.Prefix;
        var key = string.IsNullOrEmpty(prefix) ? fileName : $"{prefix.TrimEnd('/')}/{fileName}";

        fileProcessingQueue.Enqueue(
            new FileProcessingTask(
                dto.File,
                bucketName,
                key,
                dto.IsVideoProcNecessary,
                dto.Destination,
                dto.BaseUrl
            )
        );

        return new FileInfoDto(bucketName, "Processing...");
    }

    public async Task<FileData> GetFileAsync(GetFileInfoDto dto)
    {
        var bucketName = dto.BucketName;
        var key = dto.Key;

        try
        {
            var data = await bucketRepository.DownloadFileAsync(bucketName, key);

            return data;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new InternalServerErrorException("Failed to get file", ex);
        }
    }

    public FileInfoDto UpdateFile(UpdateFileInfoDto dto)
    {
        var bucketName = dto.BucketName;

        fileProcessingQueue.Enqueue(
            new FileProcessingTask(
                dto.File,
                bucketName,
                dto.Key,
                dto.IsVideoProcNecessary,
                dto.Destination,
                dto.BaseUrl
            )
        );

        return new FileInfoDto(bucketName, "Processing...");
    }

    public async Task DeleteFileAsync(DeleteFileInfoDto dto)
    {
        try
        {
            await bucketRepository.DeleteFileAsync(dto.BucketName, dto.Key);
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new InternalServerErrorException("Failed to delete file", ex);
        }
    }
}
