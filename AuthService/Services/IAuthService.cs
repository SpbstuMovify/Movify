using AuthMicroservice.Dtos;
using AuthMicroservice.Utils;

namespace AuthMicroservice.Services;

public interface IAuthService
{
    LoginUserResponseDto LoginUser(LoginUserRequestDto loginUserRequest);
    RegisterUserResponseDto RegisterUser(RegisterUserRequestDto registerUserRequest);
    Task<ValidateTokenResponseDto> ValidateTokenAsync(ValidateTokenRequestDto validateTokenRequest);
}
