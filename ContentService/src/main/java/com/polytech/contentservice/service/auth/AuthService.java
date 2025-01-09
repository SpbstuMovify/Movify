package com.polytech.contentservice.service.auth;

import com.polytech.contentservice.common.Role;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.login.UserLoginResponseDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
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
  UserRegistrationResponseDto registerUser(UserRegisterDto userDto);

  /**
   * Восстановление пароля пользователя.
   *
   * @param userDto данные для сброса пароля
   * @return данные пользователя + новый токен
   */
  UserRegistrationResponseDto resetUserPassword(UserRegisterDto userDto);

  /**
   * Метод для получения токена.
   *
   * @param userDto данные пользователя для аунтефикации
   * @param ip      IP адрес пользователя
   * @return данные пользователя + токен
   */
  UserLoginResponseDto login(UserDto userDto, String ip);

  /**
   * Метод по проверке токена.
   *
   * @param userDto информация о пользователе для валидации
   */
  void checkTokenIsValid(String token, Role role);
}
