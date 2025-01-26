using System.ComponentModel.DataAnnotations;

namespace AuthService.Utils.Configuration;

public class GrpcClientOptions
{
    [Required(ErrorMessage = "Media Service URL is required.")]
    public required string ContentServiceUrl { get; set; }
}
