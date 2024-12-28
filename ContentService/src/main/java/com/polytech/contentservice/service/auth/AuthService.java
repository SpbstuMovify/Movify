package com.polytech.contentservice.service.auth;

import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.login.UserLoginResponseDto;
import com.polytech.contentservice.dto.user.register.UserRegistrationResponseDto;

/**
 * Описание для бизнес слоя по аунтефикации пользователя.
 */
public interface AuthService {
  /**
   * Регистрация пользователя.
   *
   * @param userDto данные для регистрации
   * @return данные пользователя + токен
   */
  UserRegistrationResponseDto registerUser(UserDto userDto);

  /**
   * Метод для получения токена.
   *
   * @param userDto данные пользователя для аунтефикации
   * @return данные пользователя + токен
   */
  UserLoginResponseDto login(UserDto userDto);

  /**
   * Метод по валидации токена.
   *
   * @param userDto информация о пользователе для валидации
   * @return валиден ли токен
   */
  boolean isTokenValid(UserDto userDto);
}
