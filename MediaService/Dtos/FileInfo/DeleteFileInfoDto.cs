namespace MediaService.Dtos.FileInfo;

public class DeleteFileInfoDto
{
    public string BucketName { get; set; } = null!;
    public string Key { get; set; } = null!;
}