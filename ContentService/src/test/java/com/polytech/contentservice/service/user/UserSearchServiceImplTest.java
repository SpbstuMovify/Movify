package com.polytech.contentservice.service.user;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.common.strategy.SearchUserStrategy;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.mapper.UserMapper;
import java.util.Map;
import java.util.UUID;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class UserSearchServiceImplTest {
  @Mock
  private Map<String, SearchUserStrategy> searchUserStrategyMap;
  @Mock
  private UserMapper userMapper;
  @InjectMocks
  private UserSearchServiceImpl userSearchService;

  private UserSearchDto userSearchDto;
  private UserDto userDto;
  private SearchUserStrategy searchUserStrategy;

  @BeforeEach
  void setUp() {
    userSearchDto = getUserSearchDto();
    userDto = UserDto.builder().userId(UUID.randomUUID()).login("a").build();
    searchUserStrategy = mock(SearchUserStrategy.class);
    when(searchUserStrategy.findUser(any(UserSearchDto.class))).thenReturn(getUserEntity());
    when(userMapper.convertToUserDto(any(User.class))).thenReturn(userDto);
    when(searchUserStrategyMap.get(anyString())).thenReturn(searchUserStrategy);
  }

  @Test
  void testFindUser_validSearch() {
    UserSearchType userSearchType = UserSearchType.LOGIN;
    UserDto result = userSearchService.findUser(userSearchType, userSearchDto);

    assertNotNull(result, "The result should not be null");
    assertEquals(userDto, result, "The returned UserDto should match the mocked UserDto");
    verify(searchUserStrategy, times(1)).findUser(userSearchDto);
    verify(userMapper, times(1)).convertToUserDto(any(User.class));
  }

  @Test
  void testFindUser_emptySearchDto() {
    UserSearchDto emptySearchDto = UserSearchDto.builder()
        .build();

    UserDto result = userSearchService.findUser(UserSearchType.LOGIN, emptySearchDto);

    assertNotNull(result, "The result should not be null");
    verify(searchUserStrategy, times(1)).findUser(emptySearchDto);
    verify(userMapper, times(1)).convertToUserDto(any(User.class));
  }

  private static UserSearchDto getUserSearchDto() {
    return UserSearchDto.builder()
        .searchType(UserSearchType.LOGIN)
        .login("a")
        .build();
  }

  private static User getUserEntity() {
    return User.builder()
        .id(UUID.randomUUID())
        .login("a")
        .build();
  }
}
