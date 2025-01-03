namespace MediaService.Dtos.FileInfo;

public class UpdateFileInfoDto
{
    public string BucketName { get; set; } = null!;
    public string Key { get; set; } = null!;
    public bool IsVideoProcNecessary { get; set; }
}