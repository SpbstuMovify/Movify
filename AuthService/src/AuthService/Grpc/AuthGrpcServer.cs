using Grpc.Core;

using AuthService.Services;
using AuthService.Dtos;

namespace AuthService.Grpc;

public class AuthGrpcServer(
    ILogger<AuthGrpcServer> logger,
    IAuthService authService
) : Movify.AuthService.AuthServiceBase
{
    public override Task<Movify.LoginUserResponse> LoginUser(
        Movify.LoginUserRequest request,
        ServerCallContext context
    )
    {
        logger.LogInformation("LoginUser request received");

        var response = authService.LoginUser(
            new LoginUserRequestDto
            {
                Email = request.Email,
                Role = request.Role,
                Password = request.Password,
                PwdHash = request.PasswordHash,
                Salt = request.PasswordSalt
            }
        );

        return Task.FromResult(new Movify.LoginUserResponse { Token = response.Token });
    }

    public override Task<Movify.RegisterUserResponse> RegisterUser(
        Movify.RegisterUserRequest request,
        ServerCallContext context
    )
    {
        logger.LogInformation("RegisterUser request received");

        var response = authService.RegisterUser(
            new RegisterUserRequestDto
            {
                Email = request.Email,
                Role = request.Role,
                Password = request.Password
            }
        );

        return Task.FromResult(
            new Movify.RegisterUserResponse
            {
                Token = response.Token,
                PasswordHash = response.PwdHash,
                PasswordSalt = response.Salt
            }
        );
    }

    public override async Task<Movify.ValidationTokenResponse> ValidateToken(
        Movify.ValidationTokenRequest request,
        ServerCallContext context
    )
    {
        logger.LogInformation("ValidateToken request received");
        var response = await authService.ValidateTokenAsync(new ValidateTokenRequestDto { Token = request.Token });

        return new Movify.ValidationTokenResponse
        {
            Email = response.Email,
            Role = response.Role
        };
    }
}
