package com.polytech.contentservice.dto;

import com.fasterxml.jackson.annotation.JsonProperty;
import io.swagger.v3.oas.annotations.media.Schema;
import java.time.LocalDate;
import lombok.Builder;

/**
 * Сущность для пользователя нашего сервиса
 * @param firstName Имя пользователя
 * @param lastName Фамилия пользователя
 * @param login Логин пользователя
 * @param email Электронная почта пользователя
 * @param phone Номер телефона пользователя
 * @param birthday День рождения пользователя
 */
@Builder
@Schema(
        description = "Сущность для пользователя нашего сервиса"
)
public record UserDto(
        @Schema(
                description = "Имя пользователя",
                example = "Эрдем"
        )
        @JsonProperty(value = "first_name")
        String firstName,
        @Schema(
                description = "Фамилия пользователя",
                example = "Истаев"
        )
        @JsonProperty(value = "last_name")
        String lastName,
        @Schema(
                description = "Логин пользователя",
                example = "Greed"
        )
        String login,
        @Schema(
                description = "Электронная почта пользователя",
                example = "greed2003@mail.ru"
        )
        String email,
        @Schema(
            description = "Пароль пользователя",
            example = "VIP"
        )
        @JsonProperty(value = "password")
        String password
) {

}
