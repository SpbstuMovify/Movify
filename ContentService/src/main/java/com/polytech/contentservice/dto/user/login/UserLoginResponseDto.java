package com.polytech.contentservice.dto.user.login;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.polytech.contentservice.common.Role;
import io.swagger.v3.oas.annotations.media.Schema;
import java.util.UUID;
import lombok.Builder;

/**
 * Сущность для пользователя нашего сервиса
 *
 * @param login Логин пользователя
 * @param email Электронная почта пользователя
 * @param role  Роль пользователя
 * @param token Токен для аунтефикации
 */
@Builder
@Schema(
    description = "Сущность ответа на запрос по входу пользователя в сервис"
)
public record UserLoginResponseDto(
    @Schema(
        description = "ИД пользователя",
        example = "ee48c2c9-e39e-4014-ae1f-1ae45193d62a"
    )
    @JsonProperty(value = "user_id")
    UUID userId,
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
        description = "Роль пользоватея",
        example = "ADMIN"
    )
    @JsonProperty(value = "role")
    Role role,
    @Schema(
        description = "Токен для аунтефикации",
        example = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
    )
    @JsonProperty(value = "token")
    String token) {
}
