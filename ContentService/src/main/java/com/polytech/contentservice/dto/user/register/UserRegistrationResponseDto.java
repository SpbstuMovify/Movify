package com.polytech.contentservice.dto.user.register;

import com.fasterxml.jackson.annotation.JsonProperty;
import io.swagger.v3.oas.annotations.media.Schema;
import java.util.UUID;
import lombok.Builder;

/**
 * Сущность для ответа на запрос по регистрации пользователя.
 *
 * @param login Логин пользователя
 * @param email Электронная почта пользователя
 * @param token Токен для аунтефикации
 */
@Builder
@Schema(
    description = "Сущность ответа на запрос по регистрации пользователя в сервисе"
)
public record UserRegistrationResponseDto(
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
        description = "Токен для аунтефикации",
        example = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
    )
    @JsonProperty(value = "token")
    String token
) {
}
