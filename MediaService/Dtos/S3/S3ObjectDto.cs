namespace MediaService.Dtos.S3;

public class S3ObjectDto
{
    public string BucketName { get; set; } = null!;
    public string PresignedUrl { get; set; } = null!;
}