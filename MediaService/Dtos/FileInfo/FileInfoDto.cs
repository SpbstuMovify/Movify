namespace MediaService.Dtos.FileInfo;

public class FileInfoDto
{
    public string BucketName { get; set; } = null!;
    public string PresignedUrl { get; set; } = null!;
}