package com.polytech.contentservice.dto.content;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.polytech.contentservice.common.AgeRestriction;
import com.polytech.contentservice.common.Genre;
import io.swagger.v3.oas.annotations.media.Schema;

@Schema(description = "Сущность для фильтрации контента")
public record ContentSearchDto(
    @Schema(description = "Название фильма", example = "Первому игроку приготовиться")
    String title,
    @Schema(description = "Год создания фильма", example = "2000")
    Integer year,
    @Schema(description = "Жанр фильма", example = "BLOCKBUSTER")
    Genre genre,
    @Schema(description = "Возрастное ограничение", example = "EIGHTEEN_PLUS")
    @JsonProperty("age_restriction")
    AgeRestriction ageRestriction
) {
}
