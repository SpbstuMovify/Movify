using System.ComponentModel.DataAnnotations;

namespace AuthService.Utils.Configuration;

public class JwtOptions
{
    public static readonly string SectionName = "Jwt";
    
    [Required(ErrorMessage = "Secret is required")]
    [MinLength(16, ErrorMessage = "Secret must be at least 16 characters long")]
    public required string Secret { get; init; }
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "ExpirySeconds must be a positive integer")]
    public int ExpirySeconds { get; init; } = 3600;
}
