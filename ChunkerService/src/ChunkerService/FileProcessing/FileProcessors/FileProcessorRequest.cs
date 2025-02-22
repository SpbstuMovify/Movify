namespace ChunkerService.FileProcessing.FileProcessors;

public record FileProcessorRequest(
    string Path,
    string BucketName,
    string Key,
    string BaseUrl
);
