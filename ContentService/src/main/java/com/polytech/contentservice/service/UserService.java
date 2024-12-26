package com.polytech.contentservice.service;

import com.polytech.aps.viewvoyage.movie_service.public_interface.dto.user.UserDto;
import java.util.UUID;

public interface UserService {
    /**
     * Получение информации о пользователе по id пользователя
     * @param id Идентификатор пользователя
     * @return Информация о пользователе
     */
    UserDto getUserInformation(UUID id);

    /**
     * Получение информации о пользователе по логину пользователя в системе
     * @param login Логин в система
     * @return Информация о пользователе
     */
    UserDto getUserInformation(String login);

    /**
     * Обновление информации по пользователю
     * @param userId Идентификатор пользователя
     * @param userDto Новая информация о пользователе
     */
    void updateUserInformation(UUID userId, UserDto userDto);
}
