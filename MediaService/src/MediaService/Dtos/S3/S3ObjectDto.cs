namespace MediaService.Dtos.S3;

public record S3ObjectDto(
    string BucketName,
    string PresignedUrl
);
