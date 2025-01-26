using System.ComponentModel.DataAnnotations;

namespace MediaService.Utils.Configuration;

public class GrpcClientOptions
{
    [Required(ErrorMessage = "Auth Service URL is required.")]
    public required string AuthServiceUrl { get; set; }

    [Required(ErrorMessage = "Chunker Service URL is required.")]
    public required string ChunkerServiceUrl { get; set; }

    [Required(ErrorMessage = "Content Service URL is required.")]
    public required string ContentServiceUrl { get; set; }
}
