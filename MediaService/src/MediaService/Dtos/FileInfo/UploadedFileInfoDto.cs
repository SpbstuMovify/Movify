namespace MediaService.Dtos.FileInfo;

public record UploadedFileInfoDto(
    string ContentPath,
    string ContentType,
    string FileName
);
