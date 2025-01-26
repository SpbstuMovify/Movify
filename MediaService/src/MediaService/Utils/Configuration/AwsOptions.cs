using System.ComponentModel.DataAnnotations;

namespace MediaService.Utils.Configuration;

public class AwsOptions
{
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    [Required(ErrorMessage = "AWS Region is required.")]
    public required string Region { get; set; }
    [Required(ErrorMessage = "AWS ServiceUrl is required.")]
    public required string ServiceUrl { get; set; }
    public bool UsePathStyle { get; set; }
}
