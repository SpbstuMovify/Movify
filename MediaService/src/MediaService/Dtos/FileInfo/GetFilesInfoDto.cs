namespace MediaService.Dtos.FileInfo;

public class GetFilesInfoDto
{
    public string BucketName { get; set; } = null!;
    public string Prefix { get; set; } = null!;
}
