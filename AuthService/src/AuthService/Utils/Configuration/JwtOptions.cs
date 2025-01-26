namespace AuthService.Utils.Configuration;

public class JwtOptions
{
    public required string Secret { get; set; }
    public int ExpiryMinutes { get; set; }
}
