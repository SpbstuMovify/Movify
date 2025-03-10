using AuthService.Dtos;

namespace AuthService.Services;

public interface IAuthService
{
    LoginUserResponseDto LoginUser(LoginUserRequestDto loginUserRequest);
    RegisterUserResponseDto RegisterUser(RegisterUserRequestDto registerUserRequest);
    Task<ValidateTokenResponseDto> ValidateTokenAsync(ValidateTokenRequestDto validateTokenRequest);
}
