package com.polytech.contentservice.dto.episode;

import com.fasterxml.jackson.annotation.JsonProperty;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Builder;

import java.util.UUID;

/**
 * Сущность эпизода
 * @param id ИД эпизода
 * @param s3BucketName Ссылка на s3
 * @param episodeNumber Номер эпизода
 * @param title Название эпизода
 * @param storyline Описание эпизода
 * @param seasonNumber Номер сезона
 * @param contentId ИД контента
 */
@Builder
@Schema(
    description = "Сущность эпизода"
)
public record EpisodeDto(
    @Schema(
        description = "ИД эпизода",
        example = "fa41d538-4587-4dd2-b006-b906d19c3db0"
    )
    UUID id,
    @Schema(
        description = "Ссылка на s3",
        example = "http://{address}:{port}/api/v1/bucket/films/file/{content-uuid}/{episode-uuid}/film.mp4"
    )
    @JsonProperty("s3_bucket_name")
    String s3BucketName,
    @Schema(
        description = "Номер эпизода",
        example = "1"
    )
    @JsonProperty("episode_num")
    Integer episodeNumber,
    @Schema(
        description = "Название эпизода",
        example = "Возращение бога"
    )
    String title,
    @Schema(
        description = "Описание эпизода",
        example = "В этом эпизоде бог вернулся"
    )
    @JsonProperty("description")
    String storyline,
    @Schema(
        description = "Номер сезона",
        example = "1"
    )
    @JsonProperty("season_num")
    Integer seasonNumber,
    @Schema(
        description = "ИД контента",
        example = "cfb4e3bc-6bb6-46d8-943e-b32c0056e37f"
    )
    @JsonProperty("content_id")
    UUID contentId
) {
}
