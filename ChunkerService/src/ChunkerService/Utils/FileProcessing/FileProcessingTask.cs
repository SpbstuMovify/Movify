using ChunkerService.Repositories;

namespace ChunkerService.Utils.FileProcessing;

public record FileProcessingTask
(
    string BucketName,
    string Key,
    string BaseUrl
);
