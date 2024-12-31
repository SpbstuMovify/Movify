package com.polytech.contentservice.dto.user.search;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.validation.user.ValidUserSearchParameters;
import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.Valid;
import jakarta.validation.constraints.NotNull;
import java.util.UUID;
import lombok.Builder;

/**
 * Сущность для поиска пользователя.
 * @param login Логин пользователя
 * @param email Электронная почта пользователя
 * @param userId ИД пользователя
 * @param searchType поле по которому необходимо искать пользователя в бд
 */
@Builder
@Schema(
    description = "Сущность для поиска пользователя в сервисе"
)
@ValidUserSearchParameters
public record UserSearchDto(
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
        description = "Способ поиска пользователя",
        examples = {
            "ID",
            "LOGIN",
            "EMAIL"
        }
    )
    @Valid
    @JsonProperty(value = "search_type")
    @NotNull
    UserSearchType searchType
) {

}
