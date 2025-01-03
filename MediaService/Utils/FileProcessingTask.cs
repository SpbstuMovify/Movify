namespace MediaService.Utils;

public record FileProcessingTask(IFormFile File, string BucketName, string Key, bool IsUpdate);
