package com.polytech.contentservice.common.strategy;

import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.entity.User;

/**
 * Класс для поиска пользователя в бд по id/login/email.
 */
public interface SearchUserStrategy {
  /**
   * Метод по поиску пользователей в базе данных по id/login/email.
   * @param userDto данные, по которым надо искать
   * @return найденный в БД пользователь
   */
  User findUser(UserSearchDto userDto);
}
