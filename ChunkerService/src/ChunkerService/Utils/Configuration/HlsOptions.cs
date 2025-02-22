using System.ComponentModel.DataAnnotations;

namespace ChunkerService.Utils.Configuration;

public class HlsOptions
{
    public const string SectionName = "Hls";

    [Required(ErrorMessage = "FfmpegPath is required")]
    public required string FfmpegPath { get; init; }

    [Required(ErrorMessage = "Variants is required")]
    [MinLength(1, ErrorMessage = "At least one variant is required")]
    public required HlsVariantOptions[] Variants { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "SegmentDuration must be greater than 0")]
    public int SegmentDuration { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "AudioBitrate must be greater than 0")]
    public int AudioBitrate { get; init; }

    public string? AdditionalFfmpegArgs { get; init; }
}
