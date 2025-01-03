package com.polytech.contentservice.mapper;

import com.polytech.contentservice.dto.episode.EpisodeDto;
import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.entity.Episode;

import org.springframework.stereotype.Component;

/**
 * Маппер для Episode.
 */
@Component
public final class EpisodeMapper {
  public EpisodeDto convertToEpisodeDto(Episode episode) {
    return EpisodeDto.builder()
        .id(episode.getId())
        .s3BucketName(episode.getS3BucketName())
        .episodeNumber(episode.getEpisodeNumber())
        .seasonNumber(episode.getSeasonNumber())
        .title(episode.getTitle())
        .storyline(episode.getStoryline())
        .contentId(episode.getContent().getId())
        .build();
  }

  public Episode patchUpdate(Episode oldEpisode, EpisodeDto newDto) {
    return Episode.builder()
        .title(newDto.title() == null ? oldEpisode.getTitle() : newDto.title())
        .storyline(newDto.storyline() == null ? oldEpisode.getStoryline() : newDto.storyline())
        .seasonNumber(
            newDto.seasonNumber() == null ? oldEpisode.getSeasonNumber() : newDto.seasonNumber())
        .episodeNumber(
            newDto.episodeNumber() == null ? oldEpisode.getEpisodeNumber() : newDto.episodeNumber())
        .storyline(newDto.storyline() == null ? oldEpisode.getStoryline() : newDto.storyline())
        .s3BucketName(
            newDto.s3BucketName() == null ? oldEpisode.getS3BucketName() : newDto.s3BucketName())
        .build();
  }

  public Episode convertToEpisodeEntity(EpisodeDto episodeDto, Content content) {
    return Episode.builder()
        .s3BucketName(episodeDto.s3BucketName())
        .episodeNumber(episodeDto.episodeNumber())
        .seasonNumber(episodeDto.seasonNumber())
        .title(episodeDto.title())
        .storyline(episodeDto.storyline())
        .content(content)
        .build();
  }
}
