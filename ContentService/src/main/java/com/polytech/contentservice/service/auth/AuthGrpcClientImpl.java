package com.polytech.contentservice.service.auth;

import auth.AuthServiceGrpc;
import auth.LoginUserRequest;
import auth.LoginUserResponse;
import auth.RegisterUserRequest;
import auth.RegisterUserResponse;
import auth.ValidationTokenRequest;
import auth.ValidationTokenResponse;
import com.polytech.contentservice.config.AuthGrpcClientProperty;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.exception.LoginException;
import com.polytech.contentservice.exception.UnauthorisedException;
import io.grpc.Channel;
import io.grpc.ManagedChannelBuilder;
import org.springframework.stereotype.Service;

/**
 * Реализация {@link AuthGrpcClient}.
 */
@Service
public class AuthGrpcClientImpl implements AuthGrpcClient {
  private final AuthServiceGrpc.AuthServiceBlockingStub stub;
  private final AuthAttemptsService authAttemptsService;

  public AuthGrpcClientImpl(AuthGrpcClientProperty property,
                            AuthAttemptsService authAttemptsService) {
    Channel channel =
        ManagedChannelBuilder.forAddress(property.host(), property.port()).usePlaintext().build();
    this.stub = AuthServiceGrpc.newBlockingStub(channel);
    this.authAttemptsService = authAttemptsService;
  }

  @Override
  public LoginUserResponse sendLoginRequest(UserDto userDto, String ip) {
    try {
      if (authAttemptsService.isLoginBlocked(ip)) {
        throw new Exception("Login blocked");
      }
      LoginUserResponse loginUserResponse = stub.loginUser(LoginUserRequest.newBuilder()
          .setEmail(userDto.email())
          .setPasswordHash(userDto.passwordHash())
          .setPasswordSalt(userDto.passwordSalt())
          .setPassword(userDto.password())
          .setRole(userDto.role().toString())
          .build());
      authAttemptsService.resetLoginAttemptsByIp(ip);
      return loginUserResponse;
    } catch (Exception e) {
      if (authAttemptsService.isMaxLoginAttemptsReached(ip)) {
        throw new LoginException("Max amount of attempts is reached");
      }
      throw new LoginException("Please try again");
    }
  }

  @Override
  public RegisterUserResponse sendRegisterRequest(UserRegisterDto userDto) {
    return stub.registerUser(RegisterUserRequest.newBuilder()
        .setEmail(userDto.email())
        .setRole(userDto.role().toString())
        .setPassword(userDto.password())
        .build());
  }

  @Override
  public ValidationTokenResponse sendTokenValidationRequest(UserDto userDto) {
    try {
      return stub.validateToken(ValidationTokenRequest.newBuilder()
          .setToken(userDto.token())
          .build());
    } catch (Exception e) {
      throw new UnauthorisedException("Token is not valid");
    }
  }
}
