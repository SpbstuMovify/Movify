using AuthService.Dtos;
using AuthService.Grpc;
using AuthService.Services;
using AuthService.Utils.Exceptions;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Moq;

namespace AuthService.Tests.Grpc;

[TestSubject(typeof(AuthGrpcServer))]
public class AuthGrpcServerTest
{
    private readonly AuthGrpcServer _authGrpcServer;

    private readonly Mock<ILogger<AuthGrpcServer>> _loggerMock = new();
    private readonly Mock<IAuthService> _authServiceMock = new();

    public AuthGrpcServerTest()
    {
        _authGrpcServer = new AuthGrpcServer(
            _loggerMock.Object,
            _authServiceMock.Object
        );
    }

    [Fact]
    public async Task LoginUser_WithValidRequest_ReturnsExpectedToken()
    {
        // Arrange
        var request = new Movify.LoginUserRequest
        {
            Email = "test@example.com",
            Password = "test_password",
            PasswordSalt = "test_salt",
            PasswordHash = "test_hash",
            Role = "User"
        };
        const string expectedToken = "some_jwt_token";

        _authServiceMock
            .Setup(service => service.LoginUser(It.IsAny<LoginUserRequestDto>()))
            .Returns(
                new LoginUserResponseDto
                {
                    Token = expectedToken
                }
            );

        // Act
        var response = await _authGrpcServer.LoginUser(request, null!);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expectedToken, response.Token);

        _authServiceMock
            .Verify(
                service => service.LoginUser(
                    It.Is<LoginUserRequestDto>(
                        dto => dto.Email == request.Email
                               && dto.Password == request.Password
                               && dto.Salt == request.PasswordSalt
                               && dto.PwdHash == request.PasswordHash
                               && dto.Role == request.Role
                    )
                ),
                Times.Once
            );
    }

    [Fact]
    public async Task RegisterUser_WithValidRequest_ReturnsExpectedData()
    {
        // Arrange
        var request = new Movify.RegisterUserRequest
        {
            Email = "new@example.com",
            Password = "new_password",
            Role = "User"
        };

        var authServiceResponse = new RegisterUserResponseDto
        {
            Token = "register_token",
            PwdHash = "generated_hash",
            Salt = "generated_salt"
        };

        _authServiceMock
            .Setup(service => service.RegisterUser(It.IsAny<RegisterUserRequestDto>()))
            .Returns(authServiceResponse);

        // Act
        var response = await _authGrpcServer.RegisterUser(request, null!);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(authServiceResponse.Token, response.Token);
        Assert.Equal(authServiceResponse.PwdHash, response.PasswordHash);
        Assert.Equal(authServiceResponse.Salt, response.PasswordSalt);

        _authServiceMock
            .Verify(
                service => service.RegisterUser(
                    It.Is<RegisterUserRequestDto>(
                        dto => dto.Email == request.Email
                               && dto.Password == request.Password
                               && dto.Role == request.Role
                    )
                ),
                Times.Once
            );
    }

    [Fact]
    public async Task ValidateToken_WithValidRequest_ReturnsExpectedData()
    {
        // Arrange
        var request = new Movify.ValidationTokenRequest
        {
            Token = "valid_jwt_token"
        };

        var authServiceResponse = new ValidateTokenResponseDto
        {
            Email = "test@example.com",
            Role = "User"
        };

        _authServiceMock
            .Setup(service => service.ValidateTokenAsync(It.IsAny<ValidateTokenRequestDto>()))
            .ReturnsAsync(authServiceResponse);

        // Act
        var response = await _authGrpcServer.ValidateToken(request, null!);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(authServiceResponse.Email, response.Email);
        Assert.Equal(authServiceResponse.Role, response.Role);

        _authServiceMock
            .Verify(
                service => service.ValidateTokenAsync(
                    It.Is<ValidateTokenRequestDto>(
                        dto => dto.Token == request.Token
                    )
                ),
                Times.Once
            );
    }
    
    [Fact]
    public async Task LoginUser_WithAuthServiceThrowsException_RethrowsRpcException()
    {
        var request = new Movify.LoginUserRequest();
    
        _authServiceMock
            .Setup(s => s.LoginUser(It.IsAny<LoginUserRequestDto>()))
            .Throws(new InvalidCredentialsException("Invalid credentials!"));
        
        await Assert.ThrowsAnyAsync<Exception>(() => _authGrpcServer.LoginUser(request, null!));
    }
}
