namespace AuthService.Utils.Jwt;

public class UserClaimsData
{
    public required string Email { get; init; }
    public required string Role { get; init; }
}
