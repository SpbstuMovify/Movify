using System.ComponentModel.DataAnnotations;

namespace AuthService.Utils.Configuration;

public class GrpcClientOptions
{
    public const string SectionName = "GrpcClient";
    
    [Required(ErrorMessage = "ContentServiceUrl is required")]
    [Url(ErrorMessage = "ContentServiceUrl must be a valid URL")]
    public required string ContentServiceUrl { get; init; }
}
