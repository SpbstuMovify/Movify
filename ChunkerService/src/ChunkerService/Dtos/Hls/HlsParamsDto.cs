namespace ChunkerService.Dtos.Hls;

public record HlsParamsDto(
    string FfmpegPath,
    string InputFile,
    string OutputDirectory,
    List<HlsVariantDto> Variants,
    int SegmentDuration,
    int AudioBitrate,
    string AdditionalFfmpegArgs
);
