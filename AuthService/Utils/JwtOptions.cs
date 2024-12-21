namespace AuthMicroservice.Utils;

public class JwtOptions
{
    public string Secret { get; set; } = null!;
    public int ExpiryMinutes { get; set; }
}