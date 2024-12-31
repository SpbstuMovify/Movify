package com.polytech.contentservice.dto.content;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.polytech.contentservice.common.AgeRestriction;
import com.polytech.contentservice.common.Category;
import com.polytech.contentservice.common.Genre;
import com.polytech.contentservice.common.Quality;
import com.polytech.contentservice.dto.castmember.CastMemberDto;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Builder;

import java.util.Set;
import java.util.UUID;

/**
 * Сущность для манипулирования основаными данными фильмов и сериалов
 *
 * @param id             Идентификатор сущности контента
 * @param title          Название видео
 * @param quality        Качество видео
 * @param genre          Жанр воспроизводимового видео
 * @param category       Категория контента
 * @param ageRestriction Возрастные ограничения для просмотра
 * @param description    Описания фильма или сериала
 * @param thumbnail      Картинка на заставке к видео
 * @param publisher      Издатель произведения
 * @param castMembers    Люди, принимавшие участие в съёмке данного произведения
 */
@Builder
@Schema(description = "Сущность для манипулирования основаными данными фильмов и сериалов")
public record ContentDto(
    @Schema(description = "Идентификатор сущности контента", example = "1b12236a-aca9-47bc-95ac-f3978836de2c")
    UUID id,
    @Schema(description = "Название видео", example = "Баки Ханма")
    String title,
    @Schema(description = "Качество видео", example = "P1080")
    Quality quality,
    @Schema(description = "Жанр воспроизводимового видео", example = "ACTION_FILM")
    Genre genre,
    @Schema(description = "Категория контента", example = "ANIMATED_SERIES")
    Category category,
    @Schema(description = "Возрастные ограничения для просмотра", example = "EIGHTEEN_PLUS")
    @JsonProperty(value = "age_restriction")
    AgeRestriction ageRestriction,
    @Schema(
        description = "Описания фильма или сериала",
        example = "Баки Ханма интенсивно тренируется,чтобы превзойти отца, " +
            "который считается сильнейшим бойцом в мире. " +
            "В это же время пятеро самых жестоких заключенных-смертников совершают побег. " +
            "Ими движет только одна цель — сразиться с Баки и победить.")
    String description,
    @Schema(description = "Картинка на заставке к видео", example = "https://www.kinopoisk.ru/film/1125417/posters/")
    String thumbnail,
    @Schema(description = "Издатель произведения", example = "Netflix")
    String publisher,
    @JsonProperty(value = "cast_members")
    @Schema(description = "Люди, принимавшие участие в съёмке данного произведения")
    Set<CastMemberDto> castMembers
) {
}
