package com.polytech.contentservice.service;

import com.polytech.contentservice.entity.User;
import com.polytech.contentservice.repository.UserRepository;
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

    private UserDto convertToUserDto(User user) {
        return UserDto.builder()
                .phone(user.getPhone())
                .firstName(user.getFirstName())
                .lastName(user.getLastName())
                .email(user.getEmail())
                .createdDate(user.getCreatedDate())
                .subscription(user.getSubscriptionCode())
                .birthday(user.getBirthday())
                .endDate(user.getEndDate())
                .startDate(user.getStartDate())
                .login(user.getLogin())
                .status(user.getStatus())
                .build();
    }

    private User convertToUserDto(UserDto user) {
        return User.builder()
                .phone(user.phone())
                .firstName(user.firstName())
                .lastName(user.lastName())
                .email(user.email())
                .createdDate(user.createdDate())
                .subscriptionCode(user.subscription())
                .birthday(user.birthday())
                .endDate(user.endDate())
                .startDate(user.startDate())
                .login(user.login())
                .status(user.status())
                .build();
    }
}
