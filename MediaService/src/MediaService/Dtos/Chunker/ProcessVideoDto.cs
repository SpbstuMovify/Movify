namespace MediaService.Dtos.Chunker;

public record ProcessVideoDto(
    string BucketName,
    string Key,
    string BaseUrl
);
