namespace MediaService.Dtos.FileInfo;

public record DeleteFileInfoDto(
    string BucketName,
    string Key
);
