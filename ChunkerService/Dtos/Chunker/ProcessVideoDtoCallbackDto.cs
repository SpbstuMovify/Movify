namespace ChunkerService.Dtos.Chunker;

public class ProcessVideoDtoCallbackDto
{
    public string BucketName { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string? BaseUrl { get; set; }
    public string? Error { get; set; }
}