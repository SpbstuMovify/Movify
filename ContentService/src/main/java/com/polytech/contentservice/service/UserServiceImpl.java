package com.polytech.contentservice.service;

import com.polytech.contentservice.dto.UserDto;
import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.repository.UserRepository;
import java.time.LocalDateTime;
import java.util.UUID;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class UserServiceImpl implements UserService {
    private final UserRepository userRepository;

    @Override
    public UserDto getUserInformation(UUID id) {
        return convertToUserDto(userRepository.findById(id).orElseThrow(() -> new RuntimeException("User not found")));
    }

    @Override
    public UserDto getUserInformation(String login) {
        return convertToUserDto(userRepository.findByLogin(login).orElseThrow(() -> new RuntimeException("User not found")));
    }

    @Override
    public void updateUserInformation(UUID userId, UserDto userDto) {
        User user = convertToUserDto(userDto);
        user.setId(userId);
        userRepository.save(user);
    }

    @Override
    public UserDto saveUserInformation(UserDto userDto) {
        return convertToUserDto(userRepository.save(convertToUserDto(userDto)));
    }

    private UserDto convertToUserDto(User user) {
        return UserDto.builder()
                .phone(user.getPhone())
                .firstName(user.getFirstName())
                .lastName(user.getLastName())
                .email(user.getEmail())
                .birthday(user.getBirthday())
                .login(user.getLogin())
                .build();
    }

    private User convertToUserDto(UserDto user) {
        return User.builder()
                .phone(user.phone())
                .firstName(user.firstName())
                .lastName(user.lastName())
                .email(user.email())
                .creationDate(LocalDateTime.now())
                .updateDate(LocalDateTime.now())
                .birthday(user.birthday())
                .login(user.login())
                .password(user.password())
                .build();
    }
}
