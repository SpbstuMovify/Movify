using System.ComponentModel.DataAnnotations;

namespace AuthService.Utils.Configuration;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    
    [Required(ErrorMessage = "Secret is required")]
    [MinLength(16, ErrorMessage = "Secret must be at least 16 characters long")]
    public required string Secret { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "ExpirySeconds must be a positive integer")]
    public int ExpirySeconds { get; init; } = 3600;
}
