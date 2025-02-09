using MediaService.FileProcessing;

namespace MediaService.Dtos.FileInfo;

public record UpdateFileInfoDto(
    UploadedFileInfoDto File,
    string BucketName,
    string Key,
    bool IsVideoProcNecessary,
    FileDestination Destination,
    string BaseUrl
);
