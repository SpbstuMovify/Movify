namespace AuthMicroservice.Dtos;

public class ValidateTokenResponseDto
{
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}
