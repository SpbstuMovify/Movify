package com.polytech.contentservice.service.auth;

import com.polytech.contentservice.dto.LoginUserDto;
import com.polytech.contentservice.dto.RegisterUserDto;
import com.polytech.contentservice.dto.TokenValidationDto;
import com.polytech.contentservice.dto.user.detailed.UserDto;

/**
 * Описание для взаимодействия с сервисом авторизации.
 */
public interface AuthGrpcClientService {
  /**
   * Метод по отправке запроса для аунтефикации.
   * @param userDto - информация о пользователе
   * @return токен для дальнейшей работы в сервисе
   */
   LoginUserDto sendLoginRequest(UserDto userDto);

  /**
   * Метод по отправки запроса для регистрации пользователя в сервисе.
   *
   * @param userDto пользовательская информация
   * @return зашифрованная информация
   */
   RegisterUserDto sendRegisterRequest(UserDto userDto);

  /**
   * Метод по отправке запроса для валидации токена.
   * @param userDto токен для валидации
   * @return валиден ли токен
   */
  TokenValidationDto sendTokenValidationRequest(UserDto userDto);
}
