package com.polytech.contentservice.service.content;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.doNothing;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.episode.EpisodeDto;
import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.mapper.ContentMapper;
import com.polytech.contentservice.mapper.EpisodeMapper;
import com.polytech.contentservice.repository.ContentRepository;
import java.util.HashSet;
import java.util.List;
import java.util.Optional;
import java.util.Set;
import java.util.UUID;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageImpl;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;

@ExtendWith(MockitoExtension.class)
class ContentServiceImplTest {
  @Mock
  private ContentRepository contentRepository;
  @Mock
  private ContentMapper contentMapper;
  @Mock
  private EpisodeMapper episodeMapper;
  @InjectMocks
  private ContentServiceImpl contentService;

  private UUID contentId;
  private Content content;
  private ContentDto contentDto;

  @BeforeEach
  void setUp() {
    contentId = UUID.randomUUID();

    content = new Content();
    content.setId(contentId);
    content.setTitle("Test Content");

    contentDto = ContentDto.builder()
        .id(contentId)
        .title("Test Content")
        .build();
  }

  @Test
  void testCreateContent() {
    when(contentMapper.convertToContentEntity(contentDto)).thenReturn(content);
    when(contentRepository.save(content)).thenReturn(content);
    when(contentMapper.convertToContentDto(content)).thenReturn(contentDto);

    ContentDto savedContent = contentService.createContent(contentDto);

    assertNotNull(savedContent);
    assertEquals(contentDto.id(), savedContent.id());
    assertEquals(contentDto.title(), savedContent.title());

    verify(contentRepository, times(1)).save(content);
    verify(contentMapper, times(1)).convertToContentDto(content);
  }

  @Test
  void testUpdateContent() {
    when(contentRepository.findById(contentId)).thenReturn(Optional.of(content));
    when(contentMapper.patchUpdate(content, contentDto)).thenReturn(content);
    when(contentRepository.save(content)).thenReturn(content);

    assertDoesNotThrow(() -> contentService.updateContent(contentId, contentDto));

    verify(contentRepository, times(1)).findById(contentId);
    verify(contentMapper, times(1)).patchUpdate(content, contentDto);
    verify(contentRepository, times(1)).save(content);
  }

  @Test
  void testFindAllEpisodesForContent() {
    content.setEpisodes(new HashSet<>());

    when(contentRepository.findById(contentId)).thenReturn(Optional.of(content));

    Set<EpisodeDto> result = contentService.findAllEpisodesForContent(contentId);

    assertNotNull(result);
    assertTrue(result.isEmpty());

    verify(contentRepository, times(1)).findById(contentId);
  }

  @Test
  void testFindAllContent() {
    Pageable pageable = PageRequest.of(0, 10);
    List<Content> contentList = List.of(content);
    Page<Content> contentPage = new PageImpl<>(contentList, pageable, contentList.size());

    when(contentRepository.findAll(pageable)).thenReturn(contentPage);
    when(contentMapper.convertToListOfContentDto(contentPage)).thenReturn(List.of(contentDto));

    List<ContentDto> result = contentService.findAllContent(pageable);

    assertNotNull(result);
    assertEquals(1, result.size());
    assertEquals(contentDto.id(), result.get(0).id());

    verify(contentRepository, times(1)).findAll(pageable);
  }

  @Test
  void testFindContentById() {
    when(contentRepository.findById(contentId)).thenReturn(Optional.of(content));
    when(contentMapper.convertToContentDto(content)).thenReturn(contentDto);

    ContentDto result = contentService.findContentById(contentId);

    assertNotNull(result);
    assertEquals(contentDto.id(), result.id());

    verify(contentRepository, times(1)).findById(contentId);
    verify(contentMapper, times(1)).convertToContentDto(content);
  }

  @Test
  void testDeleteById() {
    when(contentRepository.findById(contentId)).thenReturn(Optional.of(content));
    doNothing().when(contentRepository).deleteById(contentId);

    assertDoesNotThrow(() -> contentService.deleteById(contentId));

    verify(contentRepository, times(1)).findById(contentId);
    verify(contentRepository, times(1)).deleteById(contentId);
  }
}
