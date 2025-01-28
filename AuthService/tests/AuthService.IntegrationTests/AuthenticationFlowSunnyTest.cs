using AuthService.IntegrationTests.TestBase;

using Movify;

namespace AuthService.IntegrationTests;

public class AuthenticationFlowSunnyTest : IntegrationTestBase
{
    [Fact]
    public async Task AuthenticateFlow_Sunny_RegisterLoginAndValidateToken()
    {
        SetupMockContentClientReturns(new UserRoleResponse { Role = "User" });

        // Arrange: получаем gRPC клиент для вызова методов AuthGrpcServer
        var client = GetClient();

        // --- Регистрация (Register) ---
        var registerRequest = new RegisterUserRequest
        {
            Email = "test@example.com",
            Role = "User",
            Password = "password123"
        };

        var registerResponse = await client.RegisterUserAsync(registerRequest);

        // Assert: проверяем, что регистрация вернула валидный токен, а также заполненные хэш и соль пароля
        Assert.False(string.IsNullOrWhiteSpace(registerResponse.Token));
        Assert.False(string.IsNullOrWhiteSpace(registerResponse.PasswordHash));
        Assert.False(string.IsNullOrWhiteSpace(registerResponse.PasswordSalt));

        // --- Логин (Login) ---
        // Используем данные регистрации для проведения логина
        var loginRequest = new LoginUserRequest
        {
            Email = "test@example.com",
            Role = "User",
            Password = "password123",
            PasswordHash = registerResponse.PasswordHash,
            PasswordSalt = registerResponse.PasswordSalt
        };

        var loginResponse = await client.LoginUserAsync(loginRequest);

        // Assert: проверяем, что логин вернул валидный токен
        Assert.False(string.IsNullOrWhiteSpace(loginResponse.Token));

        // --- Валидация токена (Validate Token) ---
        var validateRequest = new ValidationTokenRequest
        {
            Token = loginResponse.Token
        };

        var validateResponse = await client.ValidateTokenAsync(validateRequest);

        Assert.Equal("test@example.com", validateResponse.Email);
        Assert.Equal("User", validateResponse.Role);

        VerifyMockContentClient(new UserRoleRequest { Email = "test@example.com" });
    }
}
