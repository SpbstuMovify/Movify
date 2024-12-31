package com.polytech.contentservice.service.user;

import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.exception.NotFoundException;
import com.polytech.contentservice.mapper.UserMapper;
import com.polytech.contentservice.repository.UserRepository;
import java.util.UUID;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

/**
 * Реализация {@link UserService}
 */
@Service
@RequiredArgsConstructor
public class UserServiceImpl implements UserService {
    private final UserRepository userRepository;
    private final UserMapper userMapper;
    private final UserSearchService userSearchService;

    @Override
    public UserDto getUserById(UUID id) {
        User user = userRepository.findById(id)
            .orElseThrow(() -> new NotFoundException("User not found"));
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
}
