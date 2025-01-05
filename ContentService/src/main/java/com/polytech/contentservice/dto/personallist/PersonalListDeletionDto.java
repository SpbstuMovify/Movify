package com.polytech.contentservice.dto.personallist;

import com.fasterxml.jackson.annotation.JsonProperty;
import io.swagger.v3.oas.annotations.media.Schema;
import java.util.UUID;
import lombok.Builder;

/**
 * Сущность для удаления любимых фильмов.
 *
 * @param userId - ИД хозяина персонального листа
 * @param contentId - ИД контента который добавили
 */
@Builder
@Schema(
    description = "Сущность любимых фильмов"
)
public record PersonalListDeletionDto(
    @Schema(
        description = "ИД хозяина персонального листа",
        example = "fa41d538-4587-4dd2-b006-b906d19c3db0"
    )
    @JsonProperty("user_id")
    UUID userId,
    @Schema(
        description = "ИД контента который добавили",
        example = "f884bb0c-225e-4a6d-af6e-f87ba6c3600c"
    )
    @JsonProperty("content_id")
    UUID contentId
) {
}
