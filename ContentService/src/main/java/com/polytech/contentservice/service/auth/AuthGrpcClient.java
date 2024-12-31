package com.polytech.contentservice.service.auth;

import auth.LoginUserResponse;
import auth.RegisterUserResponse;
import auth.ValidationTokenResponse;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import java.util.Optional;

/**
 * Описание для взаимодействия с сервисом авторизации.
 */
public interface AuthGrpcClient {
  /**
   * Метод по отправке запроса для аунтефикации.
   *
   * @param userDto - информация о пользователе
   * @param ip      - IP адрес
   * @return токен для дальнейшей работы в сервисе
   */
  LoginUserResponse sendLoginRequest(UserDto userDto, String ip);

  /**
   * Метод по отправки запроса для регистрации пользователя в сервисе.
   *
   * @param userDto пользовательская информация
   * @return зашифрованная информация
   */
  RegisterUserResponse sendRegisterRequest(UserRegisterDto userDto);

  /**
   * Метод по отправке запроса для валидации токена.
   *
   * @param userDto токен для валидации
   * @return валиден ли токен
   */
  ValidationTokenResponse sendTokenValidationRequest(UserDto userDto);
}
