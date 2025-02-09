namespace MediaService.Controllers.Requests;

public record GetFilesRequest(
    string BucketName,
    string Prefix
);
