using AuthService.Grpc;
using AuthService.Dtos;
using AuthService.Utils.Jwt;
using AuthService.Utils.Encryption;
using AuthService.Utils.Exceptions;

using Grpc.Core;

using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public class AuthService(
    ILogger<AuthService> logger,
    IContentGrpcClient contentGrpcClient,
    IJwtBuilder jwtBuilder,
    IEncryptor encryptor
) : IAuthService
{
    public LoginUserResponseDto LoginUser(LoginUserRequestDto loginUserRequest)
    {
        logger.LogInformation("Login user procedure has started");

        if (loginUserRequest.PwdHash != encryptor.GetHash(loginUserRequest.Password, loginUserRequest.Salt))
        {
            const string message = "Could not authenticate user";
            logger.LogWarning($"Invalid credentials provided: {message}");
            throw new InvalidCredentialsException(message);
        }

        var token = jwtBuilder.GetToken(
            new UserClaimsData
            {
                Email = loginUserRequest.Email,
                Role = loginUserRequest.Role
            }
        );

        return new LoginUserResponseDto { Token = token };
    }

    public RegisterUserResponseDto RegisterUser(RegisterUserRequestDto registerUserRequest)
    {
        logger.LogInformation("Register user procedure has started");

        var salt = encryptor.GetSalt();
        var pwdHash = encryptor.GetHash(registerUserRequest.Password, salt);
        var token = jwtBuilder.GetToken(
            new UserClaimsData
            {
                Email = registerUserRequest.Email,
                Role = registerUserRequest.Role
            }
        );

        return new RegisterUserResponseDto
        {
            Token = token,
            PwdHash = pwdHash,
            Salt = salt
        };
    }

    public async Task<ValidateTokenResponseDto> ValidateTokenAsync(ValidateTokenRequestDto validateTokenRequest)
    {
        logger.LogInformation("Token validation procedure has started");
        
        try
        {
            var validatedData = jwtBuilder.ValidateToken(validateTokenRequest.Token);
            logger.LogDebug($"Token validated data: email[{validatedData.Email}], role[{validatedData.Role}]");

            var role = await contentGrpcClient.GetUserRoleAsync(validatedData.Email);

            if (role != validatedData.Role)
            {
                var message = $"Role: '{role}' for email: '{validatedData.Email}' received, role: '{validatedData.Role}' expected";
                logger.LogWarning($"Token validation failed: {message}");
                throw new TokenValidationException(message);
            }

            return new ValidateTokenResponseDto
            {
                Email = validatedData.Email,
                Role = validatedData.Role
            };
        }
        catch (SecurityTokenException ex)
        {
            logger.LogWarning(ex, $"Token validation failed: {ex.Message}");
            throw new TokenValidationException(ex.Message, ex);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            const string message = "User not found";
            logger.LogWarning(ex, $"Token validation failed: {message}");
            throw new TokenValidationException(message, ex);
        }
    }
}
