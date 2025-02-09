namespace MediaService.Controllers.Requests;

public record GetFileRequest(
    string BucketName,
    string Key
);
