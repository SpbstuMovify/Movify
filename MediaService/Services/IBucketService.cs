using MediaService.Dtos.Bucket;
using MediaService.Dtos.FileInfo;
using MediaService.Repositories;

namespace MediaService.Services;

public interface IBucketService
{
    Task<IList<BucketDto>> GetBucketsAsync();
    Task<BucketDto> CreateBucketAsync(CreateBucketDto createBucketDto);
    Task DeleteBucketAsync(DeleteBucketDto deleteBucketDto);
    Task<IList<FileInfoDto>> GetFilesAsync(GetFilesInfoDto getFilesInfoDto);
    FileInfoDto CreateFile(UploadedFileInfoDto file, CreateFileInfoDto createFileInfoDto);
    Task<DownloadedFile> GetFileAsync(GetFileInfoDto getFileInfoDto);
    FileInfoDto UpdateFile(UploadedFileInfoDto file, UpdateFileInfoDto updateFileInfoDto);
    Task DeleteFileAsync(DeleteFileInfoDto deleteFileInfoDto);
}