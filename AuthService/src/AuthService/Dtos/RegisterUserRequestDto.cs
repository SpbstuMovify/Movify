namespace AuthService.Dtos;

public class RegisterUserRequestDto
{
    public required string Email { get; init; }
    public required string Role { get; init; }
    public required string Password { get; init; }
}
