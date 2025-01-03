namespace MediaService.Utils.FileProcessing;

public record FileProcessingTask(IFormFile File, string BucketName, string Key, bool IsUpdate);
