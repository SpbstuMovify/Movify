package com.polytech.contentservice.service.user;

import com.polytech.contentservice.common.Role;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.exception.NotFoundException;
import com.polytech.contentservice.mapper.UserMapper;
import com.polytech.contentservice.repository.UserRepository;
import jakarta.transaction.Transactional;
import java.util.Optional;
import java.util.UUID;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

/**
 * Реализация {@link UserService}.
 */
@Service
@RequiredArgsConstructor
public class UserServiceImpl implements UserService {
  private final UserRepository userRepository;
  private final UserMapper userMapper;
  private final UserSearchService userSearchService;

  @Override
  public UserDto getUserById(UUID id) {
    User user = getUserInfoById(id);
    return userMapper.convertToUserDto(user);
  }

  @Override
  public UserDto getUserByLogin(String login) {
    User user = userRepository.findByLogin(login)
        .orElseThrow(() -> new NotFoundException("User not found"));
    return userMapper.convertToUserDto(user);
  }

  @Override
  public UserDto getUserByEmail(String email) {
    User user = userRepository.findByEmail(email)
        .orElseThrow(() -> new NotFoundException("User not found"));
    return userMapper.convertToUserDto(user);
  }

  @Override
  public Optional<UserDto> getUserByEmailNonExcept(String email) {
    return userRepository.findByEmail(email)
        .stream()
        .map(userMapper::convertToUserDto)
        .findFirst();
  }

  @Override
  @Transactional
  public void deleteById(UUID userId) {
    userRepository.findById(userId)
        .orElseThrow(() -> new NotFoundException("User not found"));
    userRepository.deleteById(userId);
  }

  @Override
  @Transactional
  public UserDto grantToAdmin(UUID userId) {
    User user = getUserInfoById(userId);
    user.setRole(Role.ADMIN);
    return userMapper.convertToUserDto(userRepository.save(user));
  }

  @Override
  public UserDto getUserInformation(UserSearchDto userDto) {
    return userSearchService.findUser(userDto.searchType(), userDto);
  }

  @Override
  public void updateUserInformation(UUID userId, UserDto userDto) {
    User user = userMapper.convertToUserDto(userDto);
    user.setId(userId);
    userRepository.save(user);
  }

  @Override
  public UserDto saveUserInformation(UserRegisterDto userDto) {
    User userToSave = userMapper.convertToUserDto(userDto);
    User user = userRepository.save(userToSave);
    return userMapper.convertToUserDto(user);
  }

  @Override
  public UserDto resetPassword(UserRegisterDto userDto) {
    User userToUpdate = userRepository.findByEmail(userDto.email())
        .orElseThrow(() -> new NotFoundException("User not found"));
    userToUpdate.setPasswordSalt(userDto.passwordSalt());
    userToUpdate.setPasswordHash(userDto.passwordHash());
    User user = userRepository.save(userToUpdate);
    return userMapper.convertToUserDto(user);
  }

  private User getUserInfoById(UUID id) {
    return userRepository.findById(id)
        .orElseThrow(() -> new NotFoundException("User not found"));
  }
}
