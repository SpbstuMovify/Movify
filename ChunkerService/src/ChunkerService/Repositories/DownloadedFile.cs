namespace ChunkerService.Repositories;

public class DownloadedFile
{
    public Stream Content { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
}
