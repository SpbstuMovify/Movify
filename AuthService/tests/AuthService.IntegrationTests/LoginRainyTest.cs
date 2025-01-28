using AuthService.IntegrationTests.TestBase;

using Grpc.Core;

using Movify;

namespace AuthService.IntegrationTests;

public class LoginRainyTest : IntegrationTestBase
{
    [Fact]
    public async Task Login_Rainy_MultipleMissingFields_RespondInvalidArgument()
    {
        // Arrange
        var client = GetClient();

        var loginRequest = new LoginUserRequest
        {
            Email = "",        // отсутствует
            Role = "",         // отсутствует
            Password = "",     // отсутствует
            PasswordHash = "", // отсутствует
            PasswordSalt = ""  // отсутствует
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => { await client.LoginUserAsync(loginRequest); });

        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains("Email is required", exception.Status.Detail);
        Assert.Contains("Role is required", exception.Status.Detail);
        Assert.Contains("Password is required", exception.Status.Detail);
        Assert.Contains("PasswordHash is required", exception.Status.Detail);
        Assert.Contains("PasswordSalt is required", exception.Status.Detail);
    }
    
    [Fact]
    public async Task Login_Rainy_InvalidCredentials_RespondUnauthenticated()
    {
        // Arrange
        var client = GetClient();
        
        var loginRequest = new LoginUserRequest
        {
            Email = "test@example.com",
            Role = "User",
            Password = "password123",
            PasswordHash = "incorrectHash",
            PasswordSalt = "someSalt"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () =>
        {
            await client.LoginUserAsync(loginRequest);
        });

        Assert.Equal(StatusCode.Unauthenticated, exception.StatusCode);
        Assert.Contains("Could not authenticate user", exception.Status.Detail);
    }
}
