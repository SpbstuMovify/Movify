package com.polytech.contentservice.mapper;

import com.polytech.contentservice.dto.UserDto;
import com.polytech.contentservice.entity.User;
import java.time.LocalDateTime;
import org.springframework.stereotype.Component;

@Component
public class UserMapper {
  public UserDto convertToUserDto(User user) {
    return UserDto.builder()
        .userId(user.getId())
        .firstName(user.getFirstName())
        .lastName(user.getLastName())
        .email(user.getEmail())
        .login(user.getLogin())
        .role(user.getRole())
        .token("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")
        .build();
  }

  public User convertToUserDto(UserDto user) {
    return User.builder()
        .firstName(user.firstName())
        .lastName(user.lastName())
        .email(user.email())
        .creationDate(LocalDateTime.now())
        .updateDate(LocalDateTime.now())
        .login(user.login())
        .password(user.password())
        .role(user.role())
        .build();
  }
}
