package com.polytech.contentservice.service.episode;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.doNothing;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.verifyNoInteractions;
import static org.mockito.Mockito.verifyNoMoreInteractions;
import static org.mockito.Mockito.when;

import com.polytech.contentservice.common.EpisodeStatus;
import com.polytech.contentservice.dto.episode.EpisodeDto;
import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.entity.Episode;
import com.polytech.contentservice.exception.NotFoundException;
import com.polytech.contentservice.mapper.EpisodeMapper;
import com.polytech.contentservice.repository.ContentRepository;
import com.polytech.contentservice.repository.EpisodeRepository;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class EpisodeServiceImplTest {
  @Mock
  private EpisodeRepository episodeRepository;

  @Mock
  private ContentRepository contentRepository;

  @Mock
  private EpisodeMapper episodeMapper;

  @InjectMocks
  private EpisodeServiceImpl episodeService;

  private UUID contentId;
  private UUID episodeId;
  private Content content;
  private Episode episode;
  private EpisodeDto episodeDto;

  @BeforeEach
  void setUp() {
    contentId = UUID.randomUUID();
    episodeId = UUID.randomUUID();

    content = new Content();
    content.setId(contentId);
    content.setTitle("Test Content");

    episode = new Episode();
    episode.setId(episodeId);
    episode.setTitle("Test Episode");
    episode.setContent(content);
    episode.setStatus(EpisodeStatus.NOT_UPLOADED);

    episodeDto = EpisodeDto.builder()
        .id(episodeId)
        .title("Test Episode")
        .status(EpisodeStatus.NOT_UPLOADED)
        .build();
  }

  @Test
  void testCreateNewEpisode() {
    when(contentRepository.findById(contentId)).thenReturn(Optional.of(content));
    when(episodeMapper.convertToEpisodeEntity(episodeDto, content)).thenReturn(episode);
    when(episodeRepository.save(episode)).thenReturn(episode);
    when(episodeMapper.convertToEpisodeDto(episode)).thenReturn(episodeDto);

    EpisodeDto savedEpisode = episodeService.createNewEpisode(contentId, episodeDto);

    assertNotNull(savedEpisode);
    assertEquals(episodeDto.id(), savedEpisode.id());
    assertEquals(EpisodeStatus.NOT_UPLOADED, savedEpisode.status());

    verify(contentRepository, times(1)).findById(contentId);
    verify(episodeRepository, times(1)).save(episode);
    verify(episodeMapper, times(1)).convertToEpisodeDto(episode);
  }

  @Test
  void testCreateNewEpisode_ContentNotFound() {
    when(contentRepository.findById(contentId)).thenReturn(Optional.empty());

    NotFoundException exception = assertThrows(NotFoundException.class, () -> {
      episodeService.createNewEpisode(contentId, episodeDto);
    });

    assertEquals("Content not found", exception.getMessage());
    verify(contentRepository, times(1)).findById(contentId);
    verifyNoInteractions(episodeRepository);
  }

  @Test
  void testUpdateEpisodeInfo() {
    when(episodeRepository.findById(episodeId)).thenReturn(Optional.of(episode));
    when(episodeMapper.patchUpdate(episode, episodeDto)).thenReturn(episode);
    when(episodeRepository.save(episode)).thenReturn(episode);

    assertDoesNotThrow(() -> episodeService.updateEpisodeInfo(episodeId, episodeDto));

    verify(episodeRepository, times(1)).findById(episodeId);
    verify(episodeMapper, times(1)).patchUpdate(episode, episodeDto);
    verify(episodeRepository, times(1)).save(episode);
  }

  @Test
  void testUpdateEpisodeInfo_EpisodeNotFound() {
    when(episodeRepository.findById(episodeId)).thenReturn(Optional.empty());

    NotFoundException exception = assertThrows(NotFoundException.class, () -> {
      episodeService.updateEpisodeInfo(episodeId, episodeDto);
    });

    assertEquals("Episode not found", exception.getMessage());
    verify(episodeRepository, times(1)).findById(episodeId);
    verifyNoMoreInteractions(episodeRepository);
  }

  @Test
  void testGetEpisodeById() {
    when(episodeRepository.findById(episodeId)).thenReturn(Optional.of(episode));
    when(episodeMapper.convertToEpisodeDto(episode)).thenReturn(episodeDto);

    EpisodeDto result = episodeService.getEpisodeById(episodeId);

    assertNotNull(result);
    assertEquals(episodeDto.id(), result.id());

    verify(episodeRepository, times(1)).findById(episodeId);
    verify(episodeMapper, times(1)).convertToEpisodeDto(episode);
  }

  @Test
  void testGetEpisodeById_NotFound() {
    when(episodeRepository.findById(episodeId)).thenReturn(Optional.empty());

    NotFoundException exception = assertThrows(NotFoundException.class, () -> {
      episodeService.getEpisodeById(episodeId);
    });

    assertEquals("Episode not found", exception.getMessage());
    verify(episodeRepository, times(1)).findById(episodeId);
  }

  @Test
  void testDeleteEpisodeById() {
    when(episodeRepository.findById(episodeId)).thenReturn(Optional.of(episode));
    doNothing().when(episodeRepository).deleteById(episodeId);

    assertDoesNotThrow(() -> episodeService.deleteEpisodeById(episodeId));

    verify(episodeRepository, times(1)).findById(episodeId);
    verify(episodeRepository, times(1)).deleteById(episodeId);
  }

  @Test
  void testDeleteEpisodeById_NotFound() {
    when(episodeRepository.findById(episodeId)).thenReturn(Optional.empty());

    NotFoundException exception = assertThrows(NotFoundException.class, () -> {
      episodeService.deleteEpisodeById(episodeId);
    });

    assertEquals("Episode not found", exception.getMessage());
    verify(episodeRepository, times(1)).findById(episodeId);
    verifyNoMoreInteractions(episodeRepository);
  }
}