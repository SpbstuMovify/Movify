package com.polytech.contentservice.service.user;

import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.dto.user.detailed.UserDto;

/**
 * Описания сервиса для поиска пользователей в бд.
 */
public interface UserSearchService {
  /**
   * Метод по поиску пользователей по userSearchType в БД.
   *
   * @param userSearchType название колонки по которой ищем
   * @param userDto - исходные данные для поиска
   * @return найденный пользователь в БД
   */
  UserDto findUser(UserSearchType userSearchType, UserSearchDto userDto);
}
