namespace MediaService.Dtos.FileInfo;

public class CreateFileInfoDto
{
    public string BucketName { get; set; } = null!;
    public string Prefix { get; set; } = null!;
    public bool IsVideoProcNecessary { get; set; }
}