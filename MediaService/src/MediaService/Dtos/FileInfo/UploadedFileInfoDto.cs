namespace MediaService.Dtos.FileInfo;

public class UploadedFileInfoDto
{
    public string ContentPath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
}
