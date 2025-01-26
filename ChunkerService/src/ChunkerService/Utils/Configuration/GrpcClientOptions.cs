using System.ComponentModel.DataAnnotations;

namespace ChunkerService.Utils.Configuration;

public class GrpcClientOptions
{
    [Required(ErrorMessage = "Media Service URL is required.")]
    public required string MediaServiceUrl { get; set; }
}
