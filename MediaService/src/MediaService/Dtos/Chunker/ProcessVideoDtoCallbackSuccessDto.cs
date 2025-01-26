namespace MediaService.Dtos.Chunker;

public class ProcessVideoDtoCallbackSuccessDto
{
    public string BucketName { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string BaseUrl { get; set; } = null!;
}
