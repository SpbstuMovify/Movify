package com.polytech.contentservice.service;

import com.polytech.contentservice.dto.LoginUserDto;
import com.polytech.contentservice.dto.RegisterUserDto;
import com.polytech.contentservice.dto.UserDto;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

/**
 * Реализация {@link AuthService}.
 */
@Service
@RequiredArgsConstructor
public class AuthServiceImpl implements AuthService {
  private final AuthGrpcClientService authGrpcClientService;

  @Override
  public UserDto registerUser(UserDto userDto) {
    RegisterUserDto registerUserDto = authGrpcClientService.sendRegisterRequest(userDto);
    return UserDto.builder()
        .token(registerUserDto.getToken())
        .userId(userDto.userId())
        .login(userDto.login())
        .email(userDto.email())
        .build();
  }

  @Override
  public UserDto login(UserDto userDto) {
    LoginUserDto loginUserDto = authGrpcClientService.sendLoginRequest(userDto);
    return UserDto.builder()
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
