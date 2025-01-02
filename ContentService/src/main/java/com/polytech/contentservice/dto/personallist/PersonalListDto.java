package com.polytech.contentservice.dto.personallist;

import com.fasterxml.jackson.annotation.JsonProperty;
import io.swagger.v3.oas.annotations.media.Schema;
import java.util.UUID;
import lombok.Builder;

@Builder
@Schema(
    description = "",
    example = ""
)
public record PersonalListDto (
    @Schema(
        description = "",
        example = ""
    )
    @JsonProperty("personal_list_id")
    UUID personalListId,
    @Schema(
        description = "",
        example = ""
    )
    @JsonProperty("user_id")
    UUID userId,
    @Schema(
        description = "",
        example = ""
    )
    @JsonProperty("content_id")
    UUID contentId
) {
}
