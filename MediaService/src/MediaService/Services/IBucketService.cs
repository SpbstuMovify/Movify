using MediaService.Dtos.Bucket;
using MediaService.Dtos.FileInfo;
using MediaService.Repositories;

namespace MediaService.Services;

public interface IBucketService
{
    Task<IList<BucketDto>> GetBucketsAsync();
    Task<BucketDto> CreateBucketAsync(CreateBucketDto dto);
    Task DeleteBucketAsync(DeleteBucketDto dto);
    Task<IList<FileInfoDto>> GetFilesAsync(GetFilesInfoDto dto);
    FileInfoDto CreateFile(CreateFileInfoDto dto);
    Task<FileData> GetFileAsync(GetFileInfoDto dto);
    FileInfoDto UpdateFile(UpdateFileInfoDto dto);
    Task DeleteFileAsync(DeleteFileInfoDto dto);
}
