package com.polytech.contentservice.service;

import com.polytech.contentservice.dto.LoginUserDto;
import com.polytech.contentservice.dto.RegisterUserDto;
import com.polytech.contentservice.dto.TokenValidationDto;
import com.polytech.contentservice.dto.UserDto;
import org.springframework.stereotype.Service;

/**
 * Реализация {@link AuthGrpcClientService}.
 */
@Service
public class AuthGrpcClientServiceImpl implements AuthGrpcClientService {

  @Override
  public LoginUserDto sendLoginRequest(UserDto userDto) {
    return LoginUserDto.builder()
        .token("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")
        .build();
  }

  @Override
  public RegisterUserDto sendRegisterRequest(UserDto userDto) {
    return RegisterUserDto.builder()
        .passwordHash("")
        .passwordSalt("")
        .token("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")
        .build();
  }

  @Override
  public TokenValidationDto sendTokenValidationRequest(UserDto userDto) {
    return TokenValidationDto.builder()
        .isValid(true)
        .build();
  }
}
