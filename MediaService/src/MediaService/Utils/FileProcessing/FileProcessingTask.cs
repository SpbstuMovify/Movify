using MediaService.Dtos.FileInfo;

namespace MediaService.Utils.FileProcessing;

public record FileProcessingTask
(
    UploadedFileInfoDto File,
    string BucketName,
    string Key,
    bool IsVideoProcNecessary, 
    FileDestination Destination,
    string BaseUrl
);
