package com.polytech.contentservice.mapper;

import static org.junit.jupiter.api.Assertions.*;

import com.polytech.contentservice.common.Role;
import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.login.UserLoginDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.entity.User;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Spy;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class UserMapperTest {
  @Spy
  private UserMapper userMapper;

  @Test
  void toFindUserDto() {
    UserLoginDto userLoginDto = UserLoginDto.builder()
        .login("aaa")
        .email("aa@mail.ru")
        .searchType(UserSearchType.LOGIN)
        .password("aaaaa111")
        .build();

    UserSearchDto actualDto = userMapper.toFindUserDto(userLoginDto);
    assertNotNull(actualDto);
    assertEquals(userLoginDto.login(), actualDto.login());
    assertEquals(userLoginDto.email(), actualDto.email());
    assertEquals(userLoginDto.searchType(), actualDto.searchType());
  }

  @Test
  void convertToUserEntity() {
    UserDto user = UserDto.builder()
        .firstName("firstName")
        .lastName("lastName")
        .email("a@mail.ru")
        .login("login")
        .role(Role.USER)
        .passwordHash("passwordHash")
        .passwordSalt("passwordSalt")
        .build();
    User actualUserEntity = userMapper.convertToUserEntity(user);
    assertEquals(user.login(), actualUserEntity.getLogin());
    assertEquals(user.email(), actualUserEntity.getEmail());
    assertEquals(user.role(), actualUserEntity.getRole());
    assertEquals(user.passwordHash(), actualUserEntity.getPasswordHash());
    assertEquals(user.passwordSalt(), actualUserEntity.getPasswordSalt());
    assertEquals(user.firstName(), actualUserEntity.getFirstName());
    assertEquals(user.lastName(), actualUserEntity.getLastName());
  }

  @Test
  void testConvertToUserEntity() {
    UserRegisterDto userRegisterDto = UserRegisterDto.builder()
        .firstName("firstName")
        .lastName("lastName")
        .email("a@mail.ru")
        .login("login")
        .role(Role.USER)
        .passwordHash("passwordHash")
        .passwordSalt("passwordSalt")
        .build();
    User actualUserEntity = userMapper.convertToUserEntity(userRegisterDto);
    assertEquals(userRegisterDto.login(), actualUserEntity.getLogin());
    assertEquals(userRegisterDto.email(), actualUserEntity.getEmail());
    assertEquals(userRegisterDto.role(), actualUserEntity.getRole());
    assertEquals(userRegisterDto.passwordHash(), actualUserEntity.getPasswordHash());
    assertEquals(userRegisterDto.passwordSalt(), actualUserEntity.getPasswordSalt());
    assertEquals(userRegisterDto.firstName(), actualUserEntity.getFirstName());
    assertEquals(userRegisterDto.lastName(), actualUserEntity.getLastName());
  }

  @Test
  void testConvertToUserDto() {
    User user = User.builder()
        .firstName("firstName")
        .lastName("lastName")
        .email("a@mail.ru")
        .login("login")
        .role(Role.USER)
        .passwordHash("passwordHash")
        .passwordSalt("passwordSalt")
        .build();
    UserDto userDto = userMapper.convertToUserDto(user);
    assertEquals(user.getLogin(), userDto.login());
    assertEquals(user.getEmail(), userDto.email());
    assertEquals(user.getRole(), userDto.role());
    assertEquals(user.getPasswordHash(), userDto.passwordHash());
    assertEquals(user.getPasswordSalt(), userDto.passwordSalt());
    assertEquals(user.getFirstName(), userDto.firstName());
    assertEquals(user.getLastName(), userDto.lastName());
  }
}
