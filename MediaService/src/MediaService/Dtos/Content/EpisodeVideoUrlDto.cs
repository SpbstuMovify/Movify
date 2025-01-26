using MediaService.Utils.FileProcessing;

namespace MediaService.Dtos.Content;

public class EpisodeVideoUrlDto
{
    public string EpisodeId { get; set; } = null!;
    public string Url { get; set; } = null!;
    public FileStatus Status { get; set; }
}
