package com.polytech.contentservice.common.strategy;

import com.polytech.contentservice.dto.UserDto;
import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

/**
 * Реализация {@link SearchUserStrategy} для id.
 */
@Component("ID")
@RequiredArgsConstructor
public class SearchUserByIdStrategy implements SearchUserStrategy {
  private final UserRepository userRepository;

  @Override
  public User findUser(UserDto userDto) {
    return userRepository.findById(userDto.userId())
        .orElseThrow(() -> new RuntimeException("User is not found"));
  }
}
