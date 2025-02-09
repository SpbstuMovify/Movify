namespace MediaService.Controllers.Requests;

public record DeleteFileRequest(
    string BucketName,
    string Key
);
