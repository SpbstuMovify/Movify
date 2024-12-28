package com.polytech.contentservice.common.strategy;

import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.exception.NotFoundException;
import com.polytech.contentservice.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

/**
 * Реализация {@link SearchUserStrategy} для login.
 */
@Component("LOGIN")
@RequiredArgsConstructor
public class SearchUserByLoginStrategy implements SearchUserStrategy {
  private final UserRepository userRepository;

  @Override
  public User findUser(UserSearchDto userDto) {
    return userRepository.findByLogin(userDto.login())
        .orElseThrow(() -> new NotFoundException("User is not found"));
  }
}
