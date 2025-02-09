namespace MediaService.Dtos.FileInfo;

public record FileInfoDto(
    string BucketName,
    string PresignedUrl
);
