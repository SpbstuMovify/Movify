using MediaService.Dtos.Bucket;
using MediaService.Dtos.FileInfo;
using MediaService.Repositories;
using MediaService.Utils.Exceptions;
using MediaService.Utils.FileProcessing;

namespace MediaService.Services;

public class BucketService(IBucketRepository bucketRepository, IFileProcessingQueue fileProcessingQueue) : IBucketService
{
    public async Task<IList<BucketDto>> GetBucketsAsync()
    {
        try
        {
            var data = await bucketRepository.GetBucketsAsync();

            return data.Select(b => new BucketDto
            {
                Name = b.BucketName,
            }).ToList();
        }
        catch (Exception ex) when (ex is not ResourceNotFoundException)
        {
            throw new InternalServerException($"Failed to get buckets", ex);
        }
    }

    public async Task<BucketDto> CreateBucketAsync(CreateBucketDto createBucketDto)
    {
        var bucketName = createBucketDto.Name;

        try
        {
            var data = await bucketRepository.CreateBucketAsync(bucketName);

            return new BucketDto
            {
                Name = data.BucketName
            };
        }
        catch (Exception ex) when (ex is not ResourceNotFoundException)
        {
            throw new InternalServerException($"Failed to get buckets", ex);
        }
    }

    public async Task DeleteBucketAsync(DeleteBucketDto deleteBucketDto)
    {
        var bucketName = deleteBucketDto.Name;

        try
        {
            await bucketRepository.DeleteBucketAsync(bucketName);
        }
        catch (Exception ex) when (ex is not ResourceNotFoundException)
        {
            throw new InternalServerException($"Failed to get buckets", ex);
        }
    }

    public async Task<IList<FileInfoDto>> GetFilesAsync(GetFilesInfoDto getFilesInfoDto)
    {
        var bucketName = getFilesInfoDto.BucketName;
        var prefix = getFilesInfoDto.Prefix;

        try
        {
            var data = await bucketRepository.GetFilesAsync(bucketName, prefix);

            return data.Select(f => new FileInfoDto
            {
                BucketName = f.BucketName,
                PresignedUrl = f.PresignedUrl

            }).ToList();
        }
        catch (Exception ex) when (ex is not ResourceNotFoundException)
        {
            throw new InternalServerException($"Failed to get buckets", ex);
        }
    }

    public FileInfoDto CreateFile(UploadedFile file, CreateFileInfoDto createFileInfoDto)
    {
        var bucketName = createFileInfoDto.BucketName;
        var prefix = createFileInfoDto.Prefix;
        var key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}";
        var isVideoProcNecessary = createFileInfoDto.IsVideoProcNecessary;
        var destination = createFileInfoDto.Destination;
        var baseUrl = createFileInfoDto.BaseUrl;

        fileProcessingQueue.Enqueue(new FileProcessingTask(file, bucketName, key, isVideoProcNecessary, destination, baseUrl));

        return new FileInfoDto
        {
            BucketName = bucketName,
            PresignedUrl = "Processing..."
        };
    }

    public async Task<DownloadedFile> GetFileAsync(GetFileInfoDto getFileInfoDto)
    {
        var bucketName = getFileInfoDto.BucketName;
        var key = getFileInfoDto.Key;

        try
        {
            var data = await bucketRepository.DownloadFileAsync(bucketName, key);

            return data;
        }
        catch (Exception ex) when (ex is not ResourceNotFoundException)
        {
            throw new InternalServerException($"Failed to get buckets", ex);
        }
    }

    public FileInfoDto UpdateFile(UploadedFile file, UpdateFileInfoDto updateFileInfoDto)
    {
        var bucketName = updateFileInfoDto.BucketName;
        var key = updateFileInfoDto.Key;
        var isVideoProcNecessary = updateFileInfoDto.IsVideoProcNecessary;
        var destination = updateFileInfoDto.Destination;
        var baseUrl = updateFileInfoDto.BaseUrl;

        fileProcessingQueue.Enqueue(new FileProcessingTask(file, bucketName, key, isVideoProcNecessary, destination, baseUrl));

        return new FileInfoDto
        {
            BucketName = bucketName,
            PresignedUrl = "Processing..."
        };
    }

    public async Task DeleteFileAsync(DeleteFileInfoDto deleteFileInfoDto)
    {
        var bucketName = deleteFileInfoDto.BucketName;
        var key = deleteFileInfoDto.Key;

        try
        {
            await bucketRepository.DeleteFileAsync(bucketName, key);
        }
        catch (Exception ex) when (ex is not ResourceNotFoundException)
        {
            throw new InternalServerException($"Failed to get buckets", ex);
        }
    }
}