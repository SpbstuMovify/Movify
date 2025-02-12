package com.polytech.contentservice.service.personallist;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.doNothing;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.verifyNoMoreInteractions;
import static org.mockito.Mockito.when;

import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.personallist.PersonalListDeletionDto;
import com.polytech.contentservice.dto.personallist.PersonalListDto;
import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.entity.PersonalList;
import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.exception.NotFoundException;
import com.polytech.contentservice.mapper.ContentMapper;
import com.polytech.contentservice.repository.ContentRepository;
import com.polytech.contentservice.repository.PersonalListRepository;
import com.polytech.contentservice.repository.UserRepository;
import java.time.LocalDateTime;
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


@ExtendWith(MockitoExtension.class)
class PersonalListServiceImplTest {
  @Mock
  private PersonalListRepository personalListRepository;

  @Mock
  private ContentRepository contentRepository;

  @Mock
  private UserRepository userRepository;

  @Mock
  private ContentMapper contentMapper;

  @InjectMocks
  private PersonalListServiceImpl personalListService;

  private UUID userId;
  private UUID contentId;
  private UUID personalListId;
  private User user;
  private Content content;
  private PersonalList personalList;
  private PersonalListDto personalListDto;
  private PersonalListDeletionDto personalListDeletionDto;

  @BeforeEach
  void setUp() {
    userId = UUID.randomUUID();
    contentId = UUID.randomUUID();
    personalListId = UUID.randomUUID();
    user = getUserEntity();
    content = getContentEntity();
    personalList = getPersonalListEntity();
    personalListDto = getPersonalListDto();
    personalListDeletionDto = new PersonalListDeletionDto(userId, contentId);
  }

  @Test
  void testAddFavoriteMovie() {
    when(userRepository.findById(any())).thenReturn(Optional.of(user));
    when(contentRepository.findById(any())).thenReturn(Optional.of(content));
    when(personalListRepository.save(any(PersonalList.class))).thenReturn(personalList);

    PersonalListDto savedPersonalList = personalListService.addFavoriteMovie(personalListDto);

    assertNotNull(savedPersonalList);
    assertEquals(personalListId, savedPersonalList.personalListId());
    assertEquals(userId, savedPersonalList.userId());
    assertEquals(contentId, savedPersonalList.contentId());

    verify(userRepository, times(1)).findById(any());
    verify(contentRepository, times(1)).findById(any());
    verify(personalListRepository, times(1)).save(any(PersonalList.class));
  }

  @Test
  void testAddFavoriteMovie_UserNotFound() {
    when(userRepository.findById(any())).thenReturn(Optional.empty());

    NotFoundException exception = assertThrows(NotFoundException.class, () -> {
      personalListService.addFavoriteMovie(personalListDto);
    });

    assertEquals("User not found", exception.getMessage());
    verify(userRepository, times(1)).findById(any());
    verifyNoMoreInteractions(contentRepository, personalListRepository);
  }

  @Test
  void testAddFavoriteMovie_ContentNotFound() {
    when(userRepository.findById(any())).thenReturn(Optional.of(user));
    when(contentRepository.findById(contentId)).thenReturn(Optional.empty());

    NotFoundException exception = assertThrows(NotFoundException.class, () -> {
      personalListService.addFavoriteMovie(personalListDto);
    });

    assertEquals("Content not found", exception.getMessage());
    verify(userRepository, times(1)).findById(any());
    verify(contentRepository, times(1)).findById(any());
    verifyNoMoreInteractions(personalListRepository);
  }

  @Test
  void testRemoveFavoriteMovie() {
    when(personalListRepository.findByUserIdAndContentId(userId, contentId))
        .thenReturn(Optional.of(personalList));
    doNothing().when(personalListRepository).deleteByUserIdAndContentId(userId, contentId);

    assertDoesNotThrow(() -> personalListService.removeFavoriteMovie(personalListDeletionDto));

    verify(personalListRepository, times(1)).findByUserIdAndContentId(userId, contentId);
    verify(personalListRepository, times(1)).deleteByUserIdAndContentId(userId, contentId);
  }

  @Test
  void testRemoveFavoriteMovie_NotFound() {
    when(personalListRepository.findByUserIdAndContentId(userId, contentId))
        .thenReturn(Optional.empty());

    NotFoundException exception = assertThrows(NotFoundException.class, () -> {
      personalListService.removeFavoriteMovie(personalListDeletionDto);
    });

    assertEquals("Personal list content does not found", exception.getMessage());
    verify(personalListRepository, times(1)).findByUserIdAndContentId(userId, contentId);
    verifyNoMoreInteractions(personalListRepository);
  }

  @Test
  void testGetFavoriteMoviesByUser() {
    ContentDto contentDto = ContentDto.builder()
        .id(contentId)
        .build();
    when(userRepository.findById(userId)).thenReturn(Optional.of(user));
    when(contentMapper.convertToContentDto(content)).thenReturn(contentDto);

    user.setPersonalList(Set.of(personalList));

    List<ContentDto> favoriteMovies = personalListService.getFavoriteMoviesByUser(userId);

    assertNotNull(favoriteMovies);
    assertEquals(1, favoriteMovies.size());
    assertEquals(contentId, favoriteMovies.get(0).id());

    verify(userRepository, times(1)).findById(userId);
    verify(contentMapper, times(1)).convertToContentDto(content);
  }

  @Test
  void testGetFavoriteMoviesByUser_UserNotFound() {
    when(userRepository.findById(userId)).thenReturn(Optional.empty());

    NotFoundException exception = assertThrows(NotFoundException.class, () -> {
      personalListService.getFavoriteMoviesByUser(userId);
    });

    assertEquals("User not found", exception.getMessage());
    verify(userRepository, times(1)).findById(userId);
  }

  private PersonalListDto getPersonalListDto() {
    return PersonalListDto.builder()
        .userId(UUID.randomUUID())
        .contentId(contentId)
        .personalListId(personalListId)
        .build();
  }

  private PersonalList getPersonalListEntity() {
    return PersonalList.builder()
        .id(personalListId)
        .user(user)
        .content(content)
        .creationDate(LocalDateTime.now())
        .build();
  }

  private Content getContentEntity() {
    return Content.builder()
        .id(contentId)
        .build();
  }

  private User getUserEntity() {
    return User.builder()
        .id(userId)
        .build();
  }
}
