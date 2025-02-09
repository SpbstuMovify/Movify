using System.ComponentModel.DataAnnotations;

namespace MediaService.Utils.Configuration;

public class GrpcClientOptions
{
    public const string SectionName = "GrpcClient";

    [Required(ErrorMessage = "AuthServiceUrl is required")]
    [Url(ErrorMessage = "AuthServiceUrl must be a valid URL")]
    public required string AuthServiceUrl { get; init; }

    [Required(ErrorMessage = "ChunkerServiceUrl is required")]
    [Url(ErrorMessage = "ChunkerServiceUrl must be a valid URL")]
    public required string ChunkerServiceUrl { get; init; }

    [Required(ErrorMessage = "ContentServiceUrl is required")]
    [Url(ErrorMessage = "ContentServiceUrl must be a valid URL")]
    public required string ContentServiceUrl { get; init; }
}
