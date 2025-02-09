using MediaService.Repositories;

namespace MediaService.FileProcessing.FileProcessors;

public record FileProcessorRequest(
    FileData File,
    string BucketName,
    string Key,
    string BaseUrl,
    bool IsVideoProcNecessary
);
