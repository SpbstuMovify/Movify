package com.polytech.contentservice.service.auth;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import com.auth.LoginUserResponse;
import com.auth.RegisterUserResponse;
import com.auth.ValidationTokenResponse;
import com.polytech.contentservice.common.Role;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.login.UserLoginResponseDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.dto.user.register.UserRegistrationResponseDto;
import com.polytech.contentservice.exception.UnauthorisedException;
import com.polytech.contentservice.service.user.UserService;
import java.util.UUID;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class AuthServiceImplTest {
  @Mock
  private AuthGrpcClient authGrpcClient;

  @Mock
  private UserService userService;

  @InjectMocks
  private AuthServiceImpl authService;

  private UserRegisterDto userRegisterDto;
  private RegisterUserResponse registerUserResponse;
  private UserDto userDto;
  private LoginUserResponse loginUserResponse;
  private ValidationTokenResponse validationTokenResponse;

  @BeforeEach
  void setUp() {
    userRegisterDto = UserRegisterDto.builder()
        .firstName("John")
        .lastName("Doe")
        .email("john@example.com")
        .login("john_doe")
        .passwordHash("hashed_password")
        .passwordSalt("salt")
        .role(Role.USER)
        .build();

    registerUserResponse = RegisterUserResponse.newBuilder()
        .setToken("test_token")
        .setPasswordHash("hashed_password")
        .setPasswordSalt("salt")
        .build();

    userDto = UserDto.builder()
        .userId(UUID.randomUUID())
        .login("john_doe")
        .email("john@example.com")
        .role(Role.USER)
        .build();

    loginUserResponse = LoginUserResponse.newBuilder()
        .setToken("login_token")
        .build();

    validationTokenResponse = ValidationTokenResponse.newBuilder()
        .setRole(Role.USER.name())
        .build();
  }

  @Test
  void testRegisterUser() {
    when(authGrpcClient.sendRegisterRequest(userRegisterDto)).thenReturn(registerUserResponse);
    when(userService.saveUserInformation(any(UserRegisterDto.class))).thenReturn(userDto);

    UserRegistrationResponseDto response = authService.registerUser(userRegisterDto);

    assertNotNull(response);
    assertEquals("test_token", response.token());
    assertEquals("john_doe", response.login());
    assertEquals("john@example.com", response.email());

    verify(authGrpcClient, times(1)).sendRegisterRequest(userRegisterDto);
    verify(userService, times(1)).saveUserInformation(any(UserRegisterDto.class));
  }

  @Test
  void testResetUserPassword() {
    when(authGrpcClient.sendRegisterRequest(userRegisterDto)).thenReturn(registerUserResponse);
    when(userService.resetPassword(any(UserRegisterDto.class))).thenReturn(userDto);

    UserRegistrationResponseDto response = authService.resetUserPassword(userRegisterDto);

    assertNotNull(response);
    assertEquals("test_token", response.token());
    assertEquals("john_doe", response.login());
    assertEquals("john@example.com", response.email());

    verify(authGrpcClient, times(1)).sendRegisterRequest(userRegisterDto);
    verify(userService, times(1)).resetPassword(any(UserRegisterDto.class));
  }

  @Test
  void testLogin() {
    String ip = "192.168.1.1";
    when(authGrpcClient.sendLoginRequest(userDto, ip)).thenReturn(loginUserResponse);

    UserLoginResponseDto response = authService.login(userDto, ip);

    assertNotNull(response);
    assertEquals("login_token", response.token());
    assertEquals("john_doe", response.login());
    assertEquals("john@example.com", response.email());
    assertEquals(Role.USER, response.role());

    verify(authGrpcClient, times(1)).sendLoginRequest(userDto, ip);
  }

  @Test
  void testCheckTokenIsValid_UserRole() {
    when(authGrpcClient.sendTokenValidationRequest(any(UserDto.class)))
        .thenReturn(validationTokenResponse);

    assertDoesNotThrow(() -> authService.checkTokenIsValid("valid_token", Role.USER));

    verify(authGrpcClient, times(1)).sendTokenValidationRequest(any(UserDto.class));
  }

  @Test
  void testCheckTokenIsValid_AdminRole() {
    ValidationTokenResponse adminResponse = ValidationTokenResponse.newBuilder()
        .setRole(Role.ADMIN.name())
        .build();
    when(authGrpcClient.sendTokenValidationRequest(any(UserDto.class)))
        .thenReturn(adminResponse);

    assertDoesNotThrow(() -> authService.checkTokenIsValid("admin_token", Role.USER));

    verify(authGrpcClient, times(1)).sendTokenValidationRequest(any(UserDto.class));
  }

  @Test
  void testCheckTokenIsValid_Unauthorized() {
    ValidationTokenResponse guestResponse = ValidationTokenResponse.newBuilder()
        .setRole("USER")
        .build();
    when(authGrpcClient.sendTokenValidationRequest(any(UserDto.class)))
        .thenReturn(guestResponse);

    assertThrows(UnauthorisedException.class,
        () -> authService.checkTokenIsValid("invalid_token", Role.ADMIN));

    verify(authGrpcClient, times(1)).sendTokenValidationRequest(any(UserDto.class));
  }

}