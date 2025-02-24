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
class SearchUserByEmailStrategyTest {
  @Mock
  private UserRepository userRepository;
  @InjectMocks
  private SearchUserByEmailStrategy strategy;

  @Test
  void findUser_thenSuccess() {
    UUID id = UUID.randomUUID();
    UserSearchDto userSearchDto = UserSearchDto.builder()
        .userId(id)
        .email("a@mail.ru")
        .login("aaa")
        .searchType(UserSearchType.EMAIL)
        .build();
    User user = User.builder()
        .email("a@mail.ru")
        .login("aaa")
        .id(id)
        .build();
    when(userRepository.findByEmail(eq(userSearchDto.email()))).thenReturn(Optional.ofNullable(user));

    User actualuser = strategy.findUser(userSearchDto);

    verify(userRepository).findByEmail(any());
    assertEquals(actualuser.getEmail(), userSearchDto.email());
  }

  @Test
  void findUser_thenNotFound() {
    UUID invalidId = UUID.randomUUID();
    UserSearchDto unknownUser = UserSearchDto.builder()
        .userId(invalidId)
        .email("iii@mail.ru")
        .login("iii")
        .searchType(UserSearchType.EMAIL)
        .build();
    when(userRepository.findByEmail(eq(unknownUser.email()))).thenReturn(Optional.empty());

    assertThrows(NotFoundException.class, () -> strategy.findUser(unknownUser));

    verify(userRepository).findByEmail(any());
  }
}
