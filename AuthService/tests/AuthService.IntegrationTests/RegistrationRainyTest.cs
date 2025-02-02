using AuthService.IntegrationTests.TestBase;

using Grpc.Core;

using Movify;

namespace AuthService.IntegrationTests;

public class RegistrationRainyTest : IntegrationTestBase
{
    [Fact]
    public async Task Register_Rainy_MultipleMissingFields_RespondInvalidArgument()
    {
        // Arrange
        var client = GetClient();

        var registerRequest = new RegisterUserRequest
        {
            Email = "",
            Role = "",
            Password = ""
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => { await client.RegisterUserAsync(registerRequest); });

        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains("Email is required", exception.Status.Detail);
        Assert.Contains("Role is required", exception.Status.Detail);
        Assert.Contains("Password is required", exception.Status.Detail);
    }
}
