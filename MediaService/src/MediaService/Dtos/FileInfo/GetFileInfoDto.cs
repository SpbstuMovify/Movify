namespace MediaService.Dtos.FileInfo;

public record GetFileInfoDto(
    string BucketName,
    string Key
);
