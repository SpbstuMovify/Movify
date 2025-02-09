using MediaService.Dtos.FileInfo;

namespace MediaService.FileProcessing;

public record FileProcessingTask(
    UploadedFileInfoDto File,
    string BucketName,
    string Key,
    bool IsVideoProcNecessary,
    FileDestination Destination,
    string BaseUrl
);
