namespace AuthService.Utils.Jwt;

public interface IJwtBuilder
{
    string GetToken(UserClaimsData userClaimsData);
    UserClaimsData ValidateToken(string token);
}
