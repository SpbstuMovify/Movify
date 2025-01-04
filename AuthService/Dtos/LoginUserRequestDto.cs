namespace AuthService.Dtos;

public class LoginUserRequestDto
{
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PwdHash { get; set; } = null!;
    public string Salt { get; set; } = null!;
}
