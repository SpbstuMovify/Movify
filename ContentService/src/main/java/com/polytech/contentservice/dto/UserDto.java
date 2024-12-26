package com.polytech.contentservice.dto;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.polytech.contentservice.common.Role;
import com.polytech.contentservice.common.UserSearchType;
import io.swagger.v3.oas.annotations.media.Schema;
import java.util.UUID;
import lombok.Builder;

/**
 * Сущность для пользователя нашего сервиса
 * @param firstName Имя пользователя
 * @param lastName Фамилия пользователя
 * @param login Логин пользователя
 * @param email Электронная почта пользователя
 * @param role Роль пользователя
 * @param token Токен для аунтефикации
 * @param searchType поле по которому необходимо искать пользователя в бд
 */
@Builder
@Schema(
        description = "Сущность для пользователя нашего сервиса"
)
public record UserDto(
    @Schema(
        description = "ИД пользователя",
        example = "ee48c2c9-e39e-4014-ae1f-1ae45193d62a"
    )
    @JsonProperty(value = "user_id")
    UUID userId,
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
        String password,
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
    String token,
    @Schema(
        description = "Способ поиска пользователя",
        examples = {
            "ID",
            "LOGIN",
            "EMAIL"
        }
    )
    @JsonProperty(value = "search_type")
    UserSearchType searchType
) {

}
