package com.polytech.contentservice.service.auth;

import com.polytech.contentservice.dto.LoginUserDto;
import com.polytech.contentservice.dto.RegisterUserDto;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.login.UserLoginResponseDto;
import com.polytech.contentservice.dto.user.register.UserRegistrationResponseDto;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

/**
 * Реализация {@link AuthService}.
 */
@Service
@RequiredArgsConstructor
public class AuthServiceImpl implements AuthService {
  private final AuthGrpcClientService authGrpcClientService;
  private final AuthAttemptsService authAttemptsService;

  @Override
  public UserRegistrationResponseDto registerUser(UserDto userDto) {
    RegisterUserDto registerUserDto = authGrpcClientService.sendRegisterRequest(userDto);
    return UserRegistrationResponseDto.builder()
        .token(registerUserDto.getToken())
        .userId(userDto.userId())
        .login(userDto.login())
        .email(userDto.email())
        .build();
  }

  @Override
  public UserLoginResponseDto login(UserDto userDto, String ip) {
    LoginUserDto loginUserDto = authGrpcClientService.sendLoginRequest(userDto);
    return UserLoginResponseDto.builder()
        .token(loginUserDto.getToken())
        .role(userDto.role())
        .userId(userDto.userId())
        .login(userDto.login())
        .email(userDto.email())
        .build();
  }

  @Override
  public boolean isTokenValid(UserDto userDto) {
    return authGrpcClientService.sendTokenValidationRequest(userDto).isValid();
  }
}
