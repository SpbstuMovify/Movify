package com.polytech.contentservice.mapper;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

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

    castMember = CastMember.builder()
        .id(UUID.randomUUID())
        .fullName("John Doe")
        .role("Actor")
        .build();

    castMemberDto = CastMemberDto.builder()
        .id(castMember.getId())
        .employeeFullName("John Doe")
        .roleName("Actor")
        .build();

    content = Content.builder()
        .id(contentId)
        .title("Test Movie")
        .year(2022)
        .description("A great movie")
        .publisher("Netflix")
        .creationDate(LocalDateTime.now())
        .updatedDate(LocalDateTime.now())
        .castMembers(Set.of(castMember))
        .build();

    contentDto = ContentDto.builder()
        .id(contentId)
        .title("Test Movie")
        .year(2022)
        .description("A great movie")
        .publisher("Netflix")
        .castMembers(Set.of(castMemberDto))
        .build();
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
        .build();

    when(castMemberMapper.convertToSetOfCastMemberEntity(any())).thenReturn(Set.of(castMember));
    Content result = contentMapper.patchUpdate(content, updatedContentDto);

    assertNotNull(result);
    assertEquals(content.getId(), result.getId());
    assertEquals("Updated Movie", result.getTitle());
    assertEquals(content.getYear(), result.getYear());
  }
}
