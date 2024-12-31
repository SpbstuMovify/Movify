package com.polytech.contentservice.dto.user.login;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.validation.user.ValidUserLoginParameters;
import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotNull;
import lombok.Builder;

/**
 * Сущность для пользователя нашего сервиса
 * @param password Пароль пользователя
 * @param login Логин пользователя
 * @param email Электронная почта пользователя
 * @param searchType поле по которому необходимо искать пользователя в бд
 */
@Schema(
    description = "Сущность для входа в сервис"
)
@Builder
@ValidUserLoginParameters
public record UserLoginDto(
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
    @NotNull
    String password,
        @Schema(
            description = "Способ поиска пользователя",
            examples = {
                "ID",
                "LOGIN",
                "EMAIL"
            }
        )
    @JsonProperty(value = "search_type")
    @NotNull
    UserSearchType searchType
) {

}
