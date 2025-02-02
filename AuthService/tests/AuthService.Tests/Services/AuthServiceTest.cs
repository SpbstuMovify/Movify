using AuthService.Dtos;
using AuthService.Grpc;
using AuthService.Utils.Encryption;
using AuthService.Utils.Exceptions;
using AuthService.Utils.Jwt;

using Grpc.Core;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Moq;

namespace AuthService.Tests.Services;

[TestSubject(typeof(AuthService.Services.AuthService))]
public class AuthServiceTest
{
    private readonly AuthService.Services.AuthService _service;

    private readonly Mock<ILogger<AuthService.Services.AuthService>> _loggerMock = new();
    private readonly Mock<IContentGrpcClient> _contentGrpcClientMock = new();
    private readonly Mock<IJwtBuilder> _jwtBuilderMock = new();
    private readonly Mock<IEncryptor> _encryptorMock = new();

    public AuthServiceTest()
    {
        _service = new AuthService.Services.AuthService(
            _loggerMock.Object,
            _contentGrpcClientMock.Object,
            _jwtBuilderMock.Object,
            _encryptorMock.Object
        );
    }

    [Fact]
    public void LoginUser_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        const string expectedToken = "expected_jwt_token";
        var request = new LoginUserRequestDto
        {
            Email = "test@example.com",
            Password = "123456",
            Salt = "test_salt",
            PwdHash = "correct_hash",
            Role = "User"
        };

        _encryptorMock
            .Setup(e => e.GetHash(request.Password, request.Salt))
            .Returns("correct_hash");

        _jwtBuilderMock
            .Setup(j => j.GetToken(It.IsAny<UserClaimsData>()))
            .Returns(expectedToken);

        var service = new AuthService.Services.AuthService(
            _loggerMock.Object,
            _contentGrpcClientMock.Object,
            _jwtBuilderMock.Object,
            _encryptorMock.Object
        );

        // Act
        var result = service.LoginUser(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedToken, result.Token);
    }

    [Fact]
    public void LoginUser_WithInvalidCredentials_ThrowsInvalidCredentialsException()
    {
        // Arrange
        var request = new LoginUserRequestDto
        {
            Email = "test@example.com",
            Password = "wrong_password",
            Salt = "test_salt",
            PwdHash = "correct_hash",
            Role = "User"
        };
        
        _encryptorMock
            .Setup(e => e.GetHash(request.Password, request.Salt))
            .Returns("some_other_hash");

        // Act & Assert
        var exception = Assert.Throws<InvalidCredentialsException>(() => _service.LoginUser(request));
        Assert.Equal("Invalid credentials provided: Could not authenticate user", exception.Message);
    }

    [Fact]
    public void RegisterUser_WithValidCredentials_ReturnsRegisterUserResponseDto()
    {
        // Arrange
        const string expectedToken = "expected_jwt_token";
        const string expectedSalt = "generated_salt";
        const string expectedHash = "generated_hash";

        var request = new RegisterUserRequestDto
        {
            Email = "newuser@example.com",
            Password = "qwerty",
            Role = "User"
        };

        _encryptorMock
            .Setup(e => e.GetSalt())
            .Returns(expectedSalt);

        _encryptorMock
            .Setup(e => e.GetHash(request.Password, expectedSalt))
            .Returns(expectedHash);

        _jwtBuilderMock
            .Setup(j => j.GetToken(It.IsAny<UserClaimsData>()))
            .Returns(expectedToken);

        // Act
        var result = _service.RegisterUser(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedToken, result.Token);
        Assert.Equal(expectedSalt, result.Salt);
        Assert.Equal(expectedHash, result.PwdHash);
    }

    [Fact]
    public async Task ValidateTokenAsync_WithValidTokenAndMatchingRole_ReturnsValidateTokenResponseDto()
    {
        // Arrange
        const string token = "valid_token";
        var validatedClaims = new UserClaimsData
        {
            Email = "test@example.com",
            Role = "User"
        };

        _jwtBuilderMock
            .Setup(j => j.ValidateToken(token))
            .Returns(validatedClaims);

        _contentGrpcClientMock
            .Setup(c => c.GetUserRoleAsync(validatedClaims.Email))
            .ReturnsAsync("User");

        var requestDto = new ValidateTokenRequestDto { Token = token };

        // Act
        var result = await _service.ValidateTokenAsync(requestDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(validatedClaims.Email, result.Email);
        Assert.Equal(validatedClaims.Role, result.Role);
    }

    [Fact]
    public async Task ValidateTokenAsync_WithValidTokenButRoleMismatch_ThrowsTokenValidationException()
    {
        // Arrange
        const string token = "valid_token";
        var validatedClaims = new UserClaimsData
        {
            Email = "test@example.com",
            Role = "Admin"
        };

        _jwtBuilderMock
            .Setup(j => j.ValidateToken(token))
            .Returns(validatedClaims);
        
        _contentGrpcClientMock
            .Setup(c => c.GetUserRoleAsync(validatedClaims.Email))
            .ReturnsAsync("User");

        var requestDto = new ValidateTokenRequestDto { Token = token };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TokenValidationException>(() => _service.ValidateTokenAsync(requestDto));
        Assert.Equal("Token validation failed: Role: 'User' for email: 'test@example.com' received, role: 'Admin' expected", exception.Message);
    }

    [Fact]
    public async Task ValidateTokenAsync_WithInvalidToken_ThrowsTokenValidationException()
    {
        // Arrange
        const string invalidToken = "invalid_token";

        _jwtBuilderMock
            .Setup(j => j.ValidateToken(invalidToken))
            .Throws(new SecurityTokenException("Invalid signature"));

        var requestDto = new ValidateTokenRequestDto { Token = invalidToken };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TokenValidationException>(() => _service.ValidateTokenAsync(requestDto));
        Assert.Equal("Token validation failed: Invalid signature", exception.Message);
    }

    [Fact]
    public async Task ValidateTokenAsync_UserNotFound_ThrowsTokenValidationException()
    {
        // Arrange
        const string token = "valid_token";
        var validatedClaims = new UserClaimsData
        {
            Email = "unknown@example.com",
            Role = "User"
        };

        _jwtBuilderMock
            .Setup(j => j.ValidateToken(token))
            .Returns(validatedClaims);
        
        _contentGrpcClientMock
            .Setup(c => c.GetUserRoleAsync(validatedClaims.Email))
            .ThrowsAsync(new RpcException(new Status(StatusCode.NotFound, "User not found")));

        var requestDto = new ValidateTokenRequestDto { Token = token };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TokenValidationException>(() => _service.ValidateTokenAsync(requestDto));
        Assert.Equal("Token validation failed: User not found", exception.Message);

    }
}
