namespace AuthService.Dtos;

public class ValidateTokenResponseDto
{
    public required string Email { get; init; }
    public required string Role { get; init; }
}
