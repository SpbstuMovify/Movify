namespace AuthMicroservice.Dtos;

public class RegisterUserResponseDto
{
    public string Token { get; set; } = null!;
    public string PwdHash { get; set; } = null!;
    public string Salt { get; set; } = null!;
}
