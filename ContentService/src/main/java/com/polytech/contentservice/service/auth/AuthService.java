package com.polytech.contentservice.service.auth;

import auth.ValidationTokenResponse;
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
   * Метод для получения токена.
   *
   * @param userDto данные пользователя для аунтефикации
   * @param ip      IP адрес пользователя
   * @return данные пользователя + токен
   */
  UserLoginResponseDto login(UserDto userDto, String ip);

  /**
   * Метод по получению данных из токена и его проверке.
   *
   * @param userDto информация о пользователе для валидации
   * @return данные из токена
   */
  ValidationTokenResponse getTokenData(UserDto userDto);
}
