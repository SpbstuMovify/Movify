package com.polytech.contentservice.dto.content;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.polytech.contentservice.common.AgeRestriction;
import com.polytech.contentservice.common.Genre;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Builder;

/**
 * Сущность для фильтрации контента.
 * @param title Название фильма
 * @param year Год создания фильма
 * @param genre Жанр фильма
 * @param ageRestriction Возрастное ограничение
 * @param pageSize Параметр количества отображаемых фильмов и сериалов на странице
 * @param pageNumber Параметр номера страницы
 */
@Builder
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
    AgeRestriction ageRestriction,
    @Schema(description = "Параметр количества отображаемых фильмов и сериалов на странице", example = "3")
    @JsonProperty("page_size")
    Integer pageSize,
    @Schema(description = "Параметр номера страницы", example = "0")
    @JsonProperty("page_number")
    Integer pageNumber
) {
}
