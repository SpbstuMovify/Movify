namespace AuthService.Dtos;

public class RegisterUserRequestDto
{
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Password { get; set; } = null!;
}
