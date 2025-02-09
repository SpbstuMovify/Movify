using MediaService.FileProcessing;

namespace MediaService.Dtos.Content;

public record EpisodeVideoUrlDto(
    string EpisodeId,
    string Url,
    FileStatus Status
);
