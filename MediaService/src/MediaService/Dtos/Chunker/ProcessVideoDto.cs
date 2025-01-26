namespace MediaService.Dtos.Chunker;

public class ProcessVideoDto
{
    public string BucketName { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string BaseUrl { get; set; } = null!;
}
