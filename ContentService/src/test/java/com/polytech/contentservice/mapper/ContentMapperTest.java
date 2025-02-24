package com.polytech.contentservice.mapper;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

import com.polytech.contentservice.common.Category;
import com.polytech.contentservice.common.Genre;
import com.polytech.contentservice.common.Quality;
import com.polytech.contentservice.dto.castmember.CastMemberDto;
import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.entity.CastMember;
import com.polytech.contentservice.entity.Content;
import java.time.LocalDateTime;
import java.util.List;
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

@ExtendWith(MockitoExtension.class)
class ContentMapperTest {
  @Mock
  private CastMemberMapper castMemberMapper;
  @InjectMocks
  private ContentMapper contentMapper;

  private Content content;
  private ContentDto contentDto;
  private CastMemberDto castMemberDto;
  private CastMember castMember;

  @BeforeEach
  void setUp() {
    UUID contentId = UUID.randomUUID();
    castMember = getCastMemberEntity();
    castMemberDto = getCastMemberDto();
    content = getContentEntity(contentId);
    contentDto = getContentDto(contentId);
  }

  @Test
  void testConvertToContentDto() {
    when(castMemberMapper.convertToSetOfCastMemberDto(any())).thenReturn(Set.of(castMemberDto));
    ContentDto result = contentMapper.convertToContentDto(content);

    assertNotNull(result);
    assertEquals(content.getId(), result.id());
    assertEquals(content.getTitle(), result.title());
    assertEquals(content.getYear(), result.year());
    assertEquals(content.getDescription(), result.description());
    assertEquals(content.getPublisher(), result.publisher());
    assertEquals(1, result.castMembers().size());
  }

  @Test
  void testConvertToContentEntity() {
    when(castMemberMapper.convertToSetOfCastMemberEntity(any())).thenReturn(Set.of(castMember));
    Content result = contentMapper.convertToContentEntity(contentDto);

    assertNotNull(result);
    assertEquals(contentDto.title(), result.getTitle());
    assertEquals(contentDto.year(), result.getYear());
    assertEquals(contentDto.description(), result.getDescription());
    assertEquals(contentDto.publisher(), result.getPublisher());
    assertEquals(1, result.getCastMembers().size());
  }

  @Test
  void testConvertToListOfContentDto() {
    Page<Content> contentPage = new PageImpl<>(List.of(content));

    when(castMemberMapper.convertToSetOfCastMemberDto(any())).thenReturn(Set.of(castMemberDto));
    List<ContentDto> result = contentMapper.convertToListOfContentDto(contentPage);

    assertNotNull(result);
    assertEquals(1, result.size());
    assertEquals(content.getId(), result.get(0).id());
  }

  @Test
  void testPatchUpdate() {
    ContentDto updatedContentDto = ContentDto.builder()
        .title("Updated Movie")
        .year(2002)
        .quality(Quality.P144)
        .genre(Genre.ACTION_FILM)
        .category(Category.MOVIE)
        .description("description")
        .publisher("publisher")
        .thumbnail("thumbnail")
        .build();

    when(castMemberMapper.convertToSetOfCastMemberEntity(any())).thenReturn(Set.of(castMember));
    Content result = contentMapper.patchUpdate(content, updatedContentDto);

    assertNotNull(result);
    assertEquals(content.getId(), result.getId());
    assertEquals(updatedContentDto.title(), result.getTitle());
    assertEquals(updatedContentDto.year(), result.getYear());
    assertEquals(updatedContentDto.quality(), result.getQuality());
    assertEquals(updatedContentDto.category(), result.getCategory());
    assertEquals(updatedContentDto.description(), result.getDescription());
    assertEquals(updatedContentDto.publisher(), result.getPublisher());
    assertEquals(updatedContentDto.thumbnail(), result.getThumbnail());
  }

  @Test
  void testPatchUpdateWithNoUpdate() {
    ContentDto updatedContentDto = ContentDto.builder()
        .build();

    when(castMemberMapper.convertToSetOfCastMemberEntity(any())).thenReturn(Set.of(castMember));
    Content result = contentMapper.patchUpdate(content, updatedContentDto);

    assertNotNull(result);
    assertEquals(content.getId(), result.getId());
    assertEquals(content.getYear(), result.getYear());
    assertEquals(content.getYear(), result.getYear());
    assertEquals(content.getQuality(), result.getQuality());
    assertEquals(content.getCategory(), result.getCategory());
    assertEquals(content.getDescription(), result.getDescription());
    assertEquals(content.getPublisher(), result.getPublisher());
    assertEquals(content.getThumbnail(), result.getThumbnail());
  }

  private ContentDto getContentDto(UUID contentId) {
    return ContentDto.builder()
        .id(contentId)
        .title("Test Movie")
        .year(2022)
        .description("A great movie")
        .publisher("Netflix")
        .castMembers(Set.of(castMemberDto))
        .build();
  }

  private Content getContentEntity(UUID contentId) {
    return Content.builder()
        .id(contentId)
        .title("Test Movie")
        .year(2022)
        .description("A great movie")
        .publisher("Netflix")
        .creationDate(LocalDateTime.now())
        .updatedDate(LocalDateTime.now())
        .castMembers(Set.of(castMember))
        .genre(Genre.ACTION_FILM)
        .thumbnail("thumbnail")
        .category(Category.MOVIE)
        .quality(Quality.P144)
        .build();
  }

  private CastMemberDto getCastMemberDto() {
    return CastMemberDto.builder()
        .id(castMember.getId())
        .employeeFullName("John Doe")
        .roleName("Actor")
        .build();
  }

  private static CastMember getCastMemberEntity() {
    return CastMember.builder()
        .id(UUID.randomUUID())
        .fullName("John Doe")
        .role("Actor")
        .build();
  }
}
