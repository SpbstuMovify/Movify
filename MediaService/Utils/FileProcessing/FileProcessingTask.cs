using MediaService.Repositories;

namespace MediaService.Utils.FileProcessing;

public record FileProcessingTask
(
    UploadedFile File,
    string BucketName,
    string Key,
    bool IsVideoProcNecessary, 
    FileDestination Destination,
    string BaseUrl
);
