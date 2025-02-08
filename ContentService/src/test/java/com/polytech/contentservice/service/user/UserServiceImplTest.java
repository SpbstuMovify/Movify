package com.polytech.contentservice.service.user;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.doNothing;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import com.polytech.contentservice.common.Role;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.exception.NotFoundException;
import com.polytech.contentservice.mapper.UserMapper;
import com.polytech.contentservice.repository.UserRepository;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class UserServiceImplTest {
  @Mock
  private UserRepository userRepository;

  @Mock
  private UserMapper userMapper;

  @InjectMocks
  private UserServiceImpl userService;

  private UUID userId;
  private User user;
  private UserDto userDto;
  private UserRegisterDto userRegisterDto;

  @BeforeEach
  void setUp() {
    userId = UUID.randomUUID();

    user = new User();
    user.setId(userId);
    user.setLogin("testUser");
    user.setEmail("test@example.com");
    user.setRole(Role.USER);

    userDto = UserDto.builder()
        .role(Role.USER)
        .login("testUser")
        .email("test@example.com")
        .build();

    userRegisterDto = UserRegisterDto.builder()
        .email("test@example.com")
        .login("testUser")
        .firstName("testUser")
        .lastName("testUser")
        .password("testUser")
        .build();
  }

  @Test
  void testGetUserById() {
    when(userRepository.findById(userId)).thenReturn(Optional.of(user));
    when(userMapper.convertToUserDto(user)).thenReturn(userDto);

    UserDto result = userService.getUserById(userId);

    assertNotNull(result);
    assertEquals(userDto, result);
    verify(userRepository, times(1)).findById(userId);
    verify(userMapper, times(1)).convertToUserDto(user);
  }

  @Test
  void testGetUserById_NotFound() {
    when(userRepository.findById(userId)).thenReturn(Optional.empty());

    NotFoundException exception = assertThrows(NotFoundException.class, () -> {
      userService.getUserById(userId);
    });

    assertEquals("User not found", exception.getMessage());
    verify(userRepository, times(1)).findById(userId);
  }

  @Test
  void testGetUserByLogin() {
    when(userRepository.findByLogin("testUser")).thenReturn(Optional.of(user));
    when(userMapper.convertToUserDto(user)).thenReturn(userDto);

    UserDto result = userService.getUserByLogin("testUser");

    assertNotNull(result);
    assertEquals(userDto, result);
    verify(userRepository, times(1)).findByLogin("testUser");
  }

  @Test
  void testGetUserByLogin_NotFound() {
    when(userRepository.findByLogin("testUser")).thenReturn(Optional.empty());

    NotFoundException exception = assertThrows(NotFoundException.class, () -> {
      userService.getUserByLogin("testUser");
    });

    assertEquals("User not found", exception.getMessage());
  }

  @Test
  void testGetUserByEmail() {
    when(userRepository.findByEmail("test@example.com")).thenReturn(Optional.of(user));
    when(userMapper.convertToUserDto(user)).thenReturn(userDto);

    UserDto result = userService.getUserByEmail("test@example.com");

    assertNotNull(result);
    assertEquals(userDto, result);
    verify(userRepository, times(1)).findByEmail("test@example.com");
  }

  @Test
  void testDeleteById() {
    when(userRepository.findById(userId)).thenReturn(Optional.of(user));
    doNothing().when(userRepository).deleteById(userId);

    assertDoesNotThrow(() -> userService.deleteById(userId));

    verify(userRepository, times(1)).findById(userId);
    verify(userRepository, times(1)).deleteById(userId);
  }

  @Test
  void testDeleteById_NotFound() {
    when(userRepository.findById(userId)).thenReturn(Optional.empty());

    assertThrows(NotFoundException.class, () -> {
      userService.deleteById(userId);
    });
  }

  @Test
  void testGrantToAdmin() {
    when(userRepository.findById(userId)).thenReturn(Optional.of(user));
    user.setRole(Role.ADMIN);
    when(userRepository.save(user)).thenReturn(user);
    when(userMapper.convertToUserDto(user)).thenReturn(UserDto.builder().userId(userId).login("testUser").email("test@example.com").role(Role.ADMIN).build());

    UserDto result = userService.grantToAdmin(userId);

    assertNotNull(result);
    assertEquals(Role.ADMIN, result.role());

    verify(userRepository, times(1)).findById(userId);
    verify(userRepository, times(1)).save(user);
  }

  @Test
  void testUpdateUserInformation() {
    when(userMapper.convertToUserEntity(userDto)).thenReturn(user);
    user.setId(userId);
    when(userRepository.save(user)).thenReturn(user);

    assertDoesNotThrow(() -> userService.updateUserInformation(userId, userDto));

    verify(userMapper, times(1)).convertToUserEntity(userDto);
    verify(userRepository, times(1)).save(user);
  }

  @Test
  void testSaveUserInformation() {
    when(userMapper.convertToUserEntity(userRegisterDto)).thenReturn(user);
    when(userRepository.save(user)).thenReturn(user);
    when(userMapper.convertToUserDto(user)).thenReturn(userDto);

    UserDto result = userService.saveUserInformation(userRegisterDto);

    assertNotNull(result);
    assertEquals(userDto, result);

    verify(userMapper, times(1)).convertToUserEntity(userRegisterDto);
    verify(userRepository, times(1)).save(user);
    verify(userMapper, times(1)).convertToUserDto(user);
  }

  @Test
  void testResetPassword() {
    when(userRepository.findByEmail("test@example.com")).thenReturn(Optional.of(user));
    user.setPasswordSalt("salt");
    user.setPasswordHash("hash");
    when(userRepository.save(user)).thenReturn(user);
    when(userMapper.convertToUserDto(user)).thenReturn(userDto);

    UserDto result = userService.resetPassword(userRegisterDto);

    assertNotNull(result);
    assertEquals(userDto, result);

    verify(userRepository, times(1)).findByEmail("test@example.com");
    verify(userRepository, times(1)).save(user);
    verify(userMapper, times(1)).convertToUserDto(user);
  }
}