namespace AuthService.Dtos;

public class LoginUserRequestDto
{
    public required string Email { get; init; }
    public required string Role { get; init; }
    public required string Password { get; init; }
    public required string PwdHash { get; init; }
    public required string Salt { get; init; }
}
