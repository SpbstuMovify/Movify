package com.polytech.contentservice.mapper;

import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.login.UserLoginDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.entity.User;
import java.time.LocalDateTime;
import org.springframework.stereotype.Component;

@Component
public class UserMapper {
  public UserSearchDto toFindUserDto(UserLoginDto userDto) {
    return UserSearchDto.builder()
        .searchType(userDto.searchType())
        .email(userDto.email())
        .login(userDto.login())
        .build();
  }

  public UserDto createUserDtoWithPassword(UserLoginDto userDto, UserDto user) {
    return UserDto.builder()
        .userId(user.userId())
        .firstName(user.firstName())
        .lastName(user.lastName())
        .email(user.email())
        .login(user.login())
        .role(user.role())
        .passwordHash(user.passwordHash())
        .passwordSalt(user.passwordSalt())
        .password(userDto.password())
        .build();
  }

  public User convertToUserEntity(UserRegisterDto user) {
    return User.builder()
        .firstName(user.firstName())
        .lastName(user.lastName())
        .email(user.email())
        .creationDate(LocalDateTime.now())
        .updateDate(LocalDateTime.now())
        .login(user.login())
        .passwordHash(user.passwordHash())
        .passwordSalt(user.passwordSalt())
        .role(user.role())
        .build();
  }

  public UserDto convertToUserDto(User user) {
    return UserDto.builder()
        .userId(user.getId())
        .firstName(user.getFirstName())
        .lastName(user.getLastName())
        .email(user.getEmail())
        .login(user.getLogin())
        .role(user.getRole())
        .passwordHash(user.getPasswordHash())
        .passwordSalt(user.getPasswordSalt())
        .build();
  }

  public User convertToUserEntity(UserDto user) {
    return User.builder()
        .firstName(user.firstName())
        .lastName(user.lastName())
        .email(user.email())
        .updateDate(LocalDateTime.now())
        .login(user.login())
        .passwordSalt(user.passwordSalt())
        .passwordHash(user.passwordHash())
        .role(user.role())
        .build();
  }
}
