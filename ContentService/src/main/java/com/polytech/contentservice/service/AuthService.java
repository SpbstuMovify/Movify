package com.polytech.contentservice.service;

import com.polytech.contentservice.dto.UserDto;

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
  UserDto registerUser(UserDto userDto);

  /**
   * Метод для получения токена.
   *
   * @param userDto данные пользователя для аунтефикации
   * @return данные пользователя + токен
   */
  UserDto login(UserDto userDto);

  /**
   * Метод по валидации токена.
   *
   * @param userDto информация о пользователе для валидации
   * @return валиден ли токен
   */
  boolean isTokenValid(UserDto userDto);
}
