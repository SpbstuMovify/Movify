package com.polytech.contentservice.service.auth;

import auth.LoginUserResponse;
import auth.RegisterUserResponse;
import auth.ValidationTokenResponse;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.login.UserLoginResponseDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.dto.user.register.UserRegistrationResponseDto;
import com.polytech.contentservice.service.user.UserService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

/**
 * Реализация {@link AuthService}.
 */
@Service
@RequiredArgsConstructor
public class AuthServiceImpl implements AuthService {
  private final AuthGrpcClient authGrpcClient;
  private final UserService userService;

  @Override
  public UserRegistrationResponseDto registerUser(UserRegisterDto userDto) {
    RegisterUserResponse registerUserDto = authGrpcClient.sendRegisterRequest(userDto);
    UserDto savedDto = userService.saveUserInformation(convertToUserDto(registerUserDto, userDto));
    return UserRegistrationResponseDto.builder()
        .token(registerUserDto.getToken())
        .userId(savedDto.userId())
        .login(savedDto.login())
        .email(savedDto.email())
        .build();
  }

  @Override
  public UserRegistrationResponseDto resetUserPassword(UserRegisterDto userDto) {
    RegisterUserResponse registerUserDto = authGrpcClient.sendRegisterRequest(userDto);
    UserDto savedDto = userService.resetPassword(convertToUserDto(registerUserDto, userDto));
    return UserRegistrationResponseDto.builder()
        .token(registerUserDto.getToken())
        .userId(savedDto.userId())
        .login(savedDto.login())
        .email(savedDto.email())
        .build();
  }

  @Override
  public UserLoginResponseDto login(UserDto userDto, String ip) {
    LoginUserResponse loginUserDto = authGrpcClient.sendLoginRequest(userDto, ip);
    return UserLoginResponseDto.builder()
        .token(loginUserDto.getToken())
        .role(userDto.role())
        .userId(userDto.userId())
        .login(userDto.login())
        .email(userDto.email())
        .build();
  }

  @Override
  public ValidationTokenResponse getTokenData(UserDto userDto) {
    return authGrpcClient.sendTokenValidationRequest(userDto);
  }

  private UserRegisterDto convertToUserDto(RegisterUserResponse response,
                                           UserRegisterDto registerUserDto) {
    return UserRegisterDto.builder()
        .firstName(registerUserDto.firstName())
        .lastName(registerUserDto.lastName())
        .email(registerUserDto.email())
        .login(registerUserDto.login())
        .passwordHash(response.getPasswordHash())
        .passwordSalt(response.getPasswordSalt())
        .role(registerUserDto.role())
        .build();
  }
}
