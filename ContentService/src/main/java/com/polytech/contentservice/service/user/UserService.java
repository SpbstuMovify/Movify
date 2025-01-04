package com.polytech.contentservice.service.user;

import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import java.util.Optional;
import java.util.UUID;

/**
 * Класс бизнес логики по управлению сущностью пользователь.
 */
public interface UserService {
  /**
   * Получение информации о пользователе по id пользователя.
   *
   * @param id Идентификатор пользователя
   * @return Информация о пользователе
   */
  UserDto getUserById(UUID id);

  /**
   * Получение информации о пользователе по логину пользователя в системе.
   *
   * @param login Логин в система
   * @return Информация о пользователе
   */
  UserDto getUserByLogin(String login);

  /**
   * Получение информации о пользователе по email пользователя в системе.
   *
   * @param email Электронный адрес
   * @return Информация о пользователе
   */
  UserDto getUserByEmail(String email);

  /**
   * Получение пользователя некоторой информации о пользователе в системе.
   *
   * @param userDto данные по которым необходимо найти пользователя
   * @return Информация о пользователе
   */
  UserDto getUserInformation(UserSearchDto userDto);

  /**
   * Обновление информации по пользователю.
   *
   * @param userId  Идентификатор пользователя
   * @param userDto Новая информация о пользователе
   */
  void updateUserInformation(UUID userId, UserDto userDto);

  /**
   * Сохранение пользователя.
   *
   * @param userDto данные пользователя для сохранения
   * @return сохранённый пользователь
   */
  UserDto saveUserInformation(UserRegisterDto userDto);

  /**
   * Сохранение для существующего пользователя.
   *
   * @param userDto данные пользователя для сохранения
   * @return сохранённый пользователь
   */
  UserDto resetPassword(UserRegisterDto userDto);

  /**
   * Получение информации о пользователе по email пользователя в системе.
   *
   * @param email Электронный адрес
   * @return Информация о пользователе
   */
  Optional<UserDto> getUserByEmailNonExcept(String email);

  /**
   * Удаление пользователя.
   *
   * @param userId ИД пользователя
   */
  void deleteById(UUID userId);

  /**
   * Повышение прав пользователя.
   *
   * @param userId ИД пользователя
   * @return обновлённые данные
   */
  UserDto grantToAdmin(UUID userId);
}
