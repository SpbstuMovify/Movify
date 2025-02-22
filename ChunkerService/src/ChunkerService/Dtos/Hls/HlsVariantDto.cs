namespace ChunkerService.Dtos.Hls;

public record HlsVariantDto(
    string Name,
    int Width,
    int Height,
    int VideoBitrate
);
