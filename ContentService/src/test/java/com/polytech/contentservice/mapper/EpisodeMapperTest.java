package com.polytech.contentservice.mapper;

import static org.junit.jupiter.api.Assertions.*;

import com.polytech.contentservice.common.EpisodeStatus;
import com.polytech.contentservice.dto.episode.EpisodeDto;
import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.entity.Episode;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Spy;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class EpisodeMapperTest {

  @Spy
  private EpisodeMapper episodeMapper;

  @Test
  void convertToEpisodeDto() {
    Episode episode = Episode.builder()
        .id(UUID.randomUUID())
        .s3BucketName("getS3BucketName")
        .episodeNumber(2)
        .seasonNumber(22)
        .title("getTitle")
        .storyline("getStoryline")
        .status(EpisodeStatus.NOT_UPLOADED)
        .content(Content.builder().id(UUID.randomUUID()).build())
        .build();
    EpisodeDto dto = episodeMapper.convertToEpisodeDto(episode);
    assertNotNull(dto);
    assertEquals(episode.getTitle(), dto.title());
    assertEquals(episode.getId(), dto.id());
    assertEquals(episode.getStoryline(), dto.storyline());
    assertEquals(episode.getEpisodeNumber(), dto.episodeNumber());
    assertEquals(episode.getS3BucketName(), dto.s3BucketName());
    assertEquals(episode.getSeasonNumber(), dto.seasonNumber());
    assertEquals(episode.getStatus(), dto.status());
    assertEquals(episode.getContent().getId(), dto.contentId());
  }

  @Test
  void patchUpdate() {
    Episode oldEpisode = Episode.builder()
        .id(UUID.randomUUID())
        .content(Content.builder().id(UUID.randomUUID()).build())
        .build();
    EpisodeDto newEpisode = EpisodeDto.builder()
        .title("title")
        .storyline("storyline")
        .seasonNumber(1)
        .episodeNumber(2)
        .storyline("storyline")
        .s3BucketName("s3BucketName")
        .status(EpisodeStatus.NOT_UPLOADED)
        .build();
    Episode updatedEpisode = episodeMapper.patchUpdate(oldEpisode, newEpisode);
    assertNotNull(updatedEpisode);
    assertEquals(newEpisode.title(), updatedEpisode.getTitle());
    assertEquals(oldEpisode.getId(), updatedEpisode.getId());
    assertEquals(newEpisode.storyline(), updatedEpisode.getStoryline());
    assertEquals(newEpisode.episodeNumber(), updatedEpisode.getEpisodeNumber());
    assertEquals(newEpisode.s3BucketName(), updatedEpisode.getS3BucketName());
    assertEquals(newEpisode.seasonNumber(), updatedEpisode.getSeasonNumber());
    assertEquals(newEpisode.status(), updatedEpisode.getStatus());
    assertEquals(oldEpisode.getContent().getId(), updatedEpisode.getContent().getId());
  }

  @Test
  void convertToEpisodeEntity() {
    Content content = Content.builder()
            .id(UUID.randomUUID())
                .build();
    EpisodeDto episodeDto = EpisodeDto.builder()
        .s3BucketName("s3BucketName")
        .episodeNumber(1)
        .seasonNumber(2)
        .title("title")
        .storyline("storyline")
        .status(EpisodeStatus.NOT_UPLOADED)
        .build();
    Episode episode = episodeMapper.convertToEpisodeEntity(episodeDto, content);
    assertNotNull(episode);
    assertEquals(episodeDto.title(), episode.getTitle());
    assertEquals(episodeDto.storyline(), episode.getStoryline());
    assertEquals(episodeDto.episodeNumber(), episode.getEpisodeNumber());
    assertEquals(episodeDto.s3BucketName(), episode.getS3BucketName());
    assertEquals(episodeDto.seasonNumber(), episode.getSeasonNumber());
    assertEquals(episodeDto.status(), episode.getStatus());
    assertEquals(content.getId(), episode.getContent().getId());
  }
}
