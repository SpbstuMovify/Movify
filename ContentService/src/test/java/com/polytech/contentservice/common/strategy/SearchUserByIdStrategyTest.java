package com.polytech.contentservice.common.strategy;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.exception.NotFoundException;
import com.polytech.contentservice.repository.UserRepository;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class SearchUserByIdStrategyTest {
  @Mock
  private UserRepository userRepository;
  @InjectMocks
  private SearchUserByIdStrategy searchUserByIdStrategy;

  @Test
  void findUser_thenSuccess() {
    UUID id = UUID.randomUUID();
    UserSearchDto userSearchDto = UserSearchDto.builder()
        .userId(id)
        .email("a@mail.ru")
        .login("aaa")
        .searchType(UserSearchType.ID)
        .build();
    User user = User.builder()
        .email("a@mail.ru")
        .login("aaa")
        .id(id)
        .build();
    when(userRepository.findById(eq(id))).thenReturn(Optional.ofNullable(user));

    User actualUser = searchUserByIdStrategy.findUser(userSearchDto);

    verify(userRepository).findById(any());
    assertEquals(actualUser.getId(), userSearchDto.userId());
  }

  @Test
  void findUser_thenNotFound() {
    UUID invalidId = UUID.randomUUID();
    UserSearchDto unknownUser = UserSearchDto.builder()
        .userId(invalidId)
        .email("iii@mail.ru")
        .login("iii")
        .searchType(UserSearchType.ID)
        .build();
    when(userRepository.findById(eq(invalidId))).thenReturn(Optional.empty());

    assertThrows(NotFoundException.class, () -> searchUserByIdStrategy.findUser(unknownUser));

    verify(userRepository).findById(any());
  }
}