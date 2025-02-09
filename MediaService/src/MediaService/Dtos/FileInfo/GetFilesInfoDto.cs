namespace MediaService.Dtos.FileInfo;

public record GetFilesInfoDto(
    string BucketName,
    string Prefix
);
