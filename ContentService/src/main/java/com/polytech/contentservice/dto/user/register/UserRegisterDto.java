package com.polytech.contentservice.dto.user.register;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.polytech.contentservice.common.Role;
import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.annotation.Nullable;
import jakarta.validation.constraints.NotNull;
import lombok.Builder;

/**
 * Сущность для пользователя нашего сервиса
 *
 * @param firstName Имя пользователя
 * @param lastName  Фамилия пользователя
 * @param login     Логин пользователя
 * @param email     Электронная почта пользователя
 * @param role      Роль пользователя
 */
@Builder
@Schema(
    description = "Сущность для регистрации пользователя в сервисе"
)
public record UserRegisterDto(
    @Schema(
        description = "Имя пользователя",
        example = "Эрдем"
    )
    @JsonProperty(value = "first_name")
    @Nullable
    String firstName,
    @Schema(
        description = "Фамилия пользователя",
        example = "Истаев"
    )
    @JsonProperty(value = "last_name")
    @Nullable
    String lastName,
    @Schema(
        description = "Логин пользователя",
        example = "Greed"
    )
    @NotNull
    String login,
    @Schema(
        description = "Электронная почта пользователя",
        example = "greed2003@mail.ru"
    )
    @NotNull
    String email,
    @Schema(
        description = "Пароль пользователя",
        example = "VIP"
    )
    @JsonProperty(value = "password")
    @NotNull
    String password,
    @JsonIgnore
    String passwordHash,
    @JsonIgnore
    String passwordSalt,
    @Schema(
        description = "Роль пользоватея",
        example = "ADMIN"
    )
    @JsonProperty(value = "role")
    @NotNull
    Role role
) {

}
