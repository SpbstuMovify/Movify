namespace MediaService.Dtos.FileInfo;

public class GetFileInfoDto
{
    public string BucketName { get; set; } = null!;
    public string Key { get; set; } = null!;
}