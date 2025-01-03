package com.polytech.contentservice.dto.personallist;

import com.fasterxml.jackson.annotation.JsonProperty;
import io.swagger.v3.oas.annotations.media.Schema;
import java.util.UUID;
import lombok.Builder;

@Builder
@Schema(
    description = "Сущность любимых фильмов"
)
public record PersonalListDto (
    @Schema(
        description = "ИД персонального листа",
        example = "cfb4e3bc-6bb6-46d8-943e-b32c0056e37f"
    )
    @JsonProperty("personal_list_id")
    UUID personalListId,
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
