using System.ComponentModel.DataAnnotations;

namespace ChunkerService.Utils.Configuration;

public class HlsVariantOptions
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Width must be greater than 0")]
    public int Width { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Height must be greater than 0")]

    public int Height { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "VideoBitrate must be greater than 0")]

    public int VideoBitrate { get; init; }
}
