using System.IdentityModel.Tokens.Jwt;
using System.Text;

using AuthService.Utils.Configuration;
using AuthService.Utils.Jwt;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Tests.Utils.Jwt;

[TestSubject(typeof(JwtBuilder))]
public class JwtBuilderTest
{
    private readonly JwtBuilder _jwtBuilder;

    private readonly JwtOptions _jwtOptions = new()
    {
        Secret = "ThisIsASuperSecureAndLongEnoughSecretKey123!",
        ExpirySeconds = 3600
    };

    public JwtBuilderTest()
    {
        var jwtOptions = Options.Create(_jwtOptions);

        _jwtBuilder = new JwtBuilder(jwtOptions);
    }

    [Fact]
    public void GetToken_WithNullOrEmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        var userClaims = new UserClaimsData
        {
            Email = "",
            Role = "Admin"
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _jwtBuilder.GetToken(userClaims));
        Assert.Equal("Email cannot be null or empty (Parameter 'userClaimsData')", exception.Message);
    }

    [Fact]
    public void GetToken_WithNullOrEmptyRole_ThrowsArgumentException()
    {
        // Arrange
        var userClaims = new UserClaimsData
        {
            Email = "test@example.com",
            Role = ""
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _jwtBuilder.GetToken(userClaims));
        Assert.Equal("Role cannot be null or empty (Parameter 'userClaimsData')", exception.Message);
    }

    [Fact]
    public void GetToken_WithValidClaims_ReturnsValidToken()
    {
        // Arrange
        var userClaims = new UserClaimsData
        {
            Email = "test@example.com",
            Role = "ADMIN"
        };

        // Act
        var token = _jwtBuilder.GetToken(userClaims);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token), "Token should not be null or empty");

        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(token));

        var jwtToken = handler.ReadJwtToken(token);
        Assert.Equal(userClaims.Email, jwtToken.Payload["userEmail"]);
        Assert.Equal(userClaims.Role, jwtToken.Payload["userRole"]);

        var expectedExpiry = DateTime.UtcNow.AddSeconds(_jwtOptions.ExpirySeconds);
        var actualExpiry = jwtToken.ValidTo;

        Assert.True((expectedExpiry - actualExpiry).TotalSeconds < 5, "Token expiration time mismatch.");
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsCorrectClaims()
    {
        // Arrange
        var originalClaims = new UserClaimsData
        {
            Email = "testuser@example.com",
            Role = "ADMIN"
        };
        var token = _jwtBuilder.GetToken(originalClaims);

        // Act
        var validatedClaims = _jwtBuilder.ValidateToken(token);

        // Assert
        Assert.Equal(originalClaims.Email, validatedClaims.Email);
        Assert.Equal(originalClaims.Role, validatedClaims.Role);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ThrowsSecurityTokenException()
    {
        // Arrange
        const string invalidToken = "abc.invalid.token";

        // Act & Assert
        var exception = Assert.Throws<SecurityTokenException>(() => _jwtBuilder.ValidateToken(invalidToken));
        Assert.Equal("Invalid token or principal could not be obtained", exception.Message);
    }

    [Fact]
    public void ValidateToken_WithMissingClaims_ThrowsSecurityTokenException()
    {
        // Arrange
        var handler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirySeconds),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };
        var securityToken = handler.CreateToken(tokenDescriptor);
        var jwtWithoutClaims = handler.WriteToken(securityToken);

        // Act & Assert
        var exception = Assert.Throws<SecurityTokenException>(() => _jwtBuilder.ValidateToken(jwtWithoutClaims));
        Assert.Equal("Required claims are missing", exception.Message);
    }

    [Fact]
    public void ValidateToken_WithExpiredToken_ThrowsSecurityTokenException()
    {
        // Arrange
        var builderWithShortExpiry = new JwtBuilder(
            Options.Create(
                new JwtOptions
                {
                    Secret = "ThisIsASuperSecureAndLongEnoughSecretKey123!",
                    ExpirySeconds = 1
                }
            )
        );

        var claims = new UserClaimsData
        {
            Email = "expired@example.com",
            Role = "USER"
        };

        var token = builderWithShortExpiry.GetToken(claims);

        Thread.Sleep(TimeSpan.FromSeconds(2));

        // Act & Assert
        var exception = Assert.Throws<SecurityTokenException>(() => builderWithShortExpiry.ValidateToken(token));
        Assert.Contains("Invalid token or principal could not be obtained", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
