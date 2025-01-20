namespace MediaService.Dtos.Chunker;

public class ProcessVideoDtoCallbackFailedDto
{
    public string BucketName { get; set; } = null!;
    public string Key { get; set; } = null!;

    public string Error { get; set; } = null!;
}