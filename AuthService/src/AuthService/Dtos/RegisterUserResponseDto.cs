namespace AuthService.Dtos;

public class RegisterUserResponseDto
{
    public required string Token { get; init; }
    public required string PwdHash { get; init; }
    public required string Salt { get; init; }
}
