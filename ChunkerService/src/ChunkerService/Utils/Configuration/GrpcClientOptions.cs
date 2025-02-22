using System.ComponentModel.DataAnnotations;

namespace ChunkerService.Utils.Configuration;

public class GrpcClientOptions
{
    public const string SectionName = "GrpcClient";

    [Required(ErrorMessage = "MediaServiceUrl is required")]
    [Url(ErrorMessage = "MediaServiceUrl must be a valid URL")]
    public required string MediaServiceUrl { get; init; }
}
