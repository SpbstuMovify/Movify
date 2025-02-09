using MediaService.FileProcessing;

namespace MediaService.Controllers.Requests;

public record UpdateFileRequest(
    IFormFile File,
    string BucketName,
    string Key,
    bool IsVideoProcNecessary,
    FileDestination Destination
);
