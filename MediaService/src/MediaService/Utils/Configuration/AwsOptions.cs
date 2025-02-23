using System.ComponentModel.DataAnnotations;

namespace MediaService.Utils.Configuration;

public class AwsOptions
{
    public const string SectionName = "Aws";

    [Required(ErrorMessage = "AccessKey is required")]
    public required string AccessKey { get; init; }

    [Required(ErrorMessage = "SecretKey is required")]
    public required string SecretKey { get; init; }

    [Required(ErrorMessage = "Region is required")]
    public required string Region { get; init; }

    [Required(ErrorMessage = "ServiceUrl is required")]
    [Url(ErrorMessage = "ServiceUrl must be a valid URL")]
    public required string ServiceUrl { get; init; }

    public bool UsePathStyle { get; init; }
}
