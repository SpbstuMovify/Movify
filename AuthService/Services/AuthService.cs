using Grpc.Core;

using AuthMicroservice.Utils;
using AuthMicroservice.Grpc;
using AuthMicroservice.Dtos;

namespace AuthMicroservice.Services;

public class AuthService(
    ILogger<AuthService> logger,
    IContentGrpcClient contentGrpcClient,
    IJwtBuilder jwtBuilder,
    IEncryptor encryptor
) : IAuthService
{
    public LoginUserResponseDto LoginUser(LoginUserRequestDto loginUserRequest)
    {
        if (loginUserRequest.PwdHash != encryptor.GetHash(loginUserRequest.Password, loginUserRequest.Salt))
        {
            throw new Exception("Could not authenticate user");
        }

        var token = jwtBuilder.GetToken(new UserClaimsData { Email = loginUserRequest.Email, Role = loginUserRequest.Role });

        return new LoginUserResponseDto { Token = token };
    }

    public RegisterUserResponseDto RegisterUser(RegisterUserRequestDto registerUserRequest)
    {
        var salt = encryptor.GetSalt();
        var pwdHash = encryptor.GetHash(registerUserRequest.Password, salt);
        var token = jwtBuilder.GetToken(new UserClaimsData { Email = registerUserRequest.Email, Role = registerUserRequest.Role });

        return new RegisterUserResponseDto { Token = token, PwdHash = pwdHash, Salt = salt };
    }

    public async Task<ValidateTokenResponseDto> ValidateTokenAsync(ValidateTokenRequestDto validateTokenRequest)
    {
        logger.LogDebug("Token validation procedure has started");

        var validatedData = jwtBuilder.ValidateToken(validateTokenRequest.Token);
        logger.LogDebug($"Token validated data: email[{validatedData.Email}], role[{validatedData.Role}]");

        var role = await contentGrpcClient.GetUserRoleAsync(validatedData.Email);

        if (role != validatedData.Role)
        {
            throw new Exception($"Role[{role}] for email[{validatedData.Email}] received, role[{validatedData.Role}] expected");
        }

        return new ValidateTokenResponseDto { Email = validatedData.Email, Role = validatedData.Role };
    }
}