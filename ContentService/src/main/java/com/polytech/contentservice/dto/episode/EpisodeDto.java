package com.polytech.contentservice.dto.episode;

import com.fasterxml.jackson.annotation.JsonProperty;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Builder;

import java.util.UUID;

@Builder
@Schema(
        description = "",
        example = ""
)
public record EpisodeDto(
        @Schema(
                description = "",
                example = ""
        )
        UUID id,
        @Schema(
                description = "",
                example = ""
        )
        @JsonProperty("s3_bucket_name")
        String s3BucketName,
        @Schema(
                description = "",
                example = ""
        )
        @JsonProperty("episode_num")
        Integer episodeNumber,
        @Schema(
                description = "",
                example = ""
        )
        String title,
        @Schema(
                description = "",
                example = ""
        )
        String thumbnail,
        @Schema(
                description = "",
                example = ""
        )
        @JsonProperty("description")
        String storyline,
        @Schema(
                description = "",
                example = ""
        )
        @JsonProperty("season_num")
        Integer seasonNumber
) {
}
