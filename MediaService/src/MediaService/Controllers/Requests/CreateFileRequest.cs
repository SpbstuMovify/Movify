using MediaService.FileProcessing;

namespace MediaService.Controllers.Requests;

public record CreateFileRequest(
    IFormFile File,
    string BucketName,
    string Prefix,
    bool IsVideoProcNecessary,
    FileDestination Destination
);
