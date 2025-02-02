using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using AuthService.IntegrationTests.TestBase;

using Grpc.Core;

using Microsoft.IdentityModel.Tokens;

using Movify;

namespace AuthService.IntegrationTests;

public class ValidationTokenRainyTest : IntegrationTestBase
{
    private const string ValidTestSecret = "9095a623-a23a-481a-aa0c-e0ad96edc103";
    private const string InvalidTestSecret = "aaaaaaaaabbbbbbbbbbccccccccccccdddd";

    [Fact]
    public async Task ValidateToken_Rainy_MultipleMissingFields_RespondInvalidArgument()
    {
        // Arrange
        var client = GetClient();

        var validateResponse = new ValidationTokenRequest
        {
            Token = ""
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => { await client.ValidateTokenAsync(validateResponse); });

        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains("Token is required", exception.Status.Detail);
    }

    [Fact]
    public async Task ValidateToken_WithInvalidToken_RespondUnauthenticated()
    {
        // Arrange
        var client = GetClient();
        const string invalidToken = "abc.invalid.token";
        var validateRequest = new ValidationTokenRequest { Token = invalidToken };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => { await client.ValidateTokenAsync(validateRequest); });

        Assert.Equal(StatusCode.Unauthenticated, exception.StatusCode);
        Assert.Contains("Invalid token or principal could not be obtained", exception.Status.Detail);
    }

    [Fact]
    public async Task ValidateToken_WithMissingClaims_RespondUnauthenticated()
    {
        // Arrange
        var client = GetClient();

        var handler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidTestSecret));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };
        var token = handler.CreateToken(tokenDescriptor);
        var tokenWithoutClaims = handler.WriteToken(token);

        var validateRequest = new ValidationTokenRequest { Token = tokenWithoutClaims };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => { await client.ValidateTokenAsync(validateRequest); });

        Assert.Equal(StatusCode.Unauthenticated, exception.StatusCode);
        Assert.Contains("Required claims are missing", exception.Status.Detail);
    }

    [Fact]
    public async Task ValidateToken_WithExpiredToken_RespondUnauthenticated()
    {
        // Arrange
        var client = GetClient();

        var handler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidTestSecret));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim("userEmail", "expired@example.com"),
                    new Claim("userRole", "USER")
                ]
            ),
            Expires = DateTime.UtcNow.AddSeconds(1),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };
        var token = handler.CreateToken(tokenDescriptor);
        var expiredToken = handler.WriteToken(token);

        var validateRequest = new ValidationTokenRequest { Token = expiredToken };

        Thread.Sleep(TimeSpan.FromSeconds(2));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => { await client.ValidateTokenAsync(validateRequest); });

        Assert.Equal(StatusCode.Unauthenticated, exception.StatusCode);
        Assert.Contains("Invalid token or principal could not be obtained", exception.Status.Detail);
    }

    [Fact]
    public async Task ValidateToken_InvalidSecret_RespondUnauthenticated()
    {
        // Arrange
        var client = GetClient();

        var handler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(InvalidTestSecret));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim("userEmail", "expired@example.com"),
                    new Claim("userRole", "USER")
                ]
            ),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };
        var token = handler.CreateToken(tokenDescriptor);
        var expiredToken = handler.WriteToken(token);

        var validateRequest = new ValidationTokenRequest { Token = expiredToken };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => { await client.ValidateTokenAsync(validateRequest); });

        Assert.Equal(StatusCode.Unauthenticated, exception.StatusCode);
        Assert.Contains("Invalid token or principal could not be obtained", exception.Status.Detail);
    }

    [Fact]
    public async Task ValidateToken_ContentServiceNotFoundResponse_RespondUnauthenticated()
    {
        // Arrange
        SetupMockContentClientThrows(new RpcException(new Status(StatusCode.NotFound, "User not found")));

        var client = GetClient();

        var handler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidTestSecret));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim("userEmail", "expired@example.com"),
                    new Claim("userRole", "USER")
                ]
            ),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };
        var token = handler.CreateToken(tokenDescriptor);
        var expiredToken = handler.WriteToken(token);

        var validateRequest = new ValidationTokenRequest { Token = expiredToken };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => { await client.ValidateTokenAsync(validateRequest); });

        Assert.Equal(StatusCode.Unauthenticated, exception.StatusCode);
        Assert.Contains("User not found", exception.Status.Detail);
    }

    [Fact]
    public async Task ValidateToken_ContentServiceInternalResponse_RespondInternal()
    {
        // Arrange
        SetupMockContentClientThrows(new RpcException(new Status(StatusCode.Internal, "Internal server error")));

        var client = GetClient();

        var handler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidTestSecret));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim("userEmail", "expired@example.com"),
                    new Claim("userRole", "USER")
                ]
            ),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };
        var token = handler.CreateToken(tokenDescriptor);
        var expiredToken = handler.WriteToken(token);

        var validateRequest = new ValidationTokenRequest { Token = expiredToken };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => { await client.ValidateTokenAsync(validateRequest); });

        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        Assert.Contains("Internal server error", exception.Status.Detail);
    }

    [Fact]
    public async Task ValidateToken_ContentServiceOkResponse_NotTheSameRole_RespondUnauthenticated()
    {
        // Arrange
        SetupMockContentClientReturns(new UserRoleResponse { Role = "USER" });

        var client = GetClient();

        var handler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidTestSecret));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim("userEmail", "expired@example.com"),
                    new Claim("userRole", "ADMIN")
                ]
            ),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };
        var token = handler.CreateToken(tokenDescriptor);
        var expiredToken = handler.WriteToken(token);

        var validateRequest = new ValidationTokenRequest { Token = expiredToken };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(async () => { await client.ValidateTokenAsync(validateRequest); });

        Assert.Equal(StatusCode.Unauthenticated, exception.StatusCode);
        Assert.Contains("Role: 'USER' for email: 'expired@example.com' received, role: 'ADMIN' expected", exception.Status.Detail);
    }
}
