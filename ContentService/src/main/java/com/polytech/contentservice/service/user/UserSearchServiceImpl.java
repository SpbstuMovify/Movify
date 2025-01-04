package com.polytech.contentservice.service.user;

import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.common.strategy.SearchUserStrategy;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.mapper.UserMapper;
import java.util.Map;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

/**
 * Реализация {@link UserSearchService}.
 */
@Service
@RequiredArgsConstructor
public class UserSearchServiceImpl implements UserSearchService {
  private final Map<String, SearchUserStrategy> searchUserStrategyMap;
  private final UserMapper userMapper;

  public UserDto findUser(UserSearchType userSearchType, UserSearchDto userDto) {
    SearchUserStrategy searchUserStrategy = searchUserStrategyMap.get(userSearchType.name());
    User user = searchUserStrategy.findUser(userDto);
    return userMapper.convertToUserDto(user);
  }
}
