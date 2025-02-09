using MediaService.FileProcessing;

namespace MediaService.Dtos.FileInfo;

public record CreateFileInfoDto(
    UploadedFileInfoDto File,
    string BucketName,
    string Prefix,
    bool IsVideoProcNecessary,
    FileDestination Destination,
    string BaseUrl
);
