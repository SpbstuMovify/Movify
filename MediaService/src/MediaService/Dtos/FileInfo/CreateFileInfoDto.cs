using MediaService.Utils.FileProcessing;

namespace MediaService.Dtos.FileInfo;

public class CreateFileInfoDto
{
    public string BucketName { get; set; } = null!;
    public string Prefix { get; set; } = null!;
    public bool IsVideoProcNecessary { get; set; }
    public FileDestination Destination { get; set; }
    public string BaseUrl { get; set; } = null!;
}
