namespace ChunkerService.Dtos.Chunker;

public record ProcessVideoCallbackDto(
    string BucketName,
    string Key,
    string? BaseUrl,
    string? Error
);
