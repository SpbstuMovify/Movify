using Grpc.Core;
using AuthMicroservice.Utils;
using AuthMicroservice.Services;
using AuthMicroservice.Dtos;

namespace AuthMicroservice.Grpc;

public class AuthGrpcServer(ILogger<AuthGrpcServer> logger, IAuthService authService) : Auth.AuthBase
{
    public override Task<LoginUserResponse> LoginUser(LoginUserRequest request, ServerCallContext context)
    {
        logger.LogInformation("LoginUser request received");

        try 
        {
            var response = authService.LoginUser(new LoginUserRequestDto{ 
                Email = request.Email, 
                Role = request.Role, 
                Password = request.Password,
                PwdHash = request.PasswordHash,
                Salt = request.PasswordSalt
            });

            return Task.FromResult(new LoginUserResponse{Token = response.Token });
        }
        catch (Exception e)
        {
            logger.LogWarning($"Something went wrong while processing LoginUser: {e.Message}");
            throw new RpcException(new Status(StatusCode.Unauthenticated, e.Message));
        }
    }

    public override Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request, ServerCallContext context)
    {
        logger.LogInformation("RegisterUser request received");

        try 
        {
            var response = authService.RegisterUser(new RegisterUserRequestDto{ 
                Email = request.Email, 
                Role = request.Role, 
                Password = request.Password
            });

            return Task.FromResult(new RegisterUserResponse{
                Token = response.Token,
                PasswordHash = response.PwdHash,
                PasswordSalt = response.Salt
            });
        }
        catch (Exception e)
        {
            logger.LogWarning($"Something went wrong while processing RegisterUser: {e.Message}");
            throw new RpcException(new Status(StatusCode.Unauthenticated, e.Message));
        }
    }

    public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        logger.LogInformation("ValidateToken request received");

        try 
        {
            var response = await authService.ValidateTokenAsync(new ValidateTokenRequestDto{ 
                Token = request.Token
            });

            return new ValidateTokenResponse{
                Email = response.Email,
                Role = response.Role
            };
        }
        catch (Exception e)
        {
            logger.LogWarning($"Something went wrong while processing ValidateToken: {e.Message}");
            throw new RpcException(new Status(StatusCode.Unauthenticated, e.Message));
        }
    }
}
