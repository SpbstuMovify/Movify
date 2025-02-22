namespace ChunkerService.FileProcessing;

public record FileProcessingTask(
    string BucketName,
    string Key,
    string BaseUrl
);
