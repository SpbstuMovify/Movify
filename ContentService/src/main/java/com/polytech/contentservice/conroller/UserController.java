package com.polytech.contentservice.conroller;

import com.polytech.contentservice.dto.UserDto;
import com.polytech.contentservice.service.UserService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.tags.Tag;
import java.util.UUID;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequiredArgsConstructor
@RequestMapping("/v1/users")
@Tag(
        name = "Контролер пользовательской информации",
        description = "Позволяет манипулировать информацией о пользователях"
)
public class UserController {
    private final UserService userService;

    @GetMapping("/{user-id}")
    @Operation(
            summary = "Получения конкретного пользователя по ID",
            description = "Позволяет получить пользовательскую информацию по заданному ID"
    )
    public UserDto getUserById(
            @Parameter(description = "Пользовательский ID, по которому будем искать информацию", example = "f7d1b2c1-cedb-4099-99d8-e4ec9302dcde")
            @PathVariable("user-id")
            UUID userId) {
        return userService.getUserInformation(userId);
    }

    @GetMapping("/{login}")
    @Operation(
            summary = "Получения конкретного пользователя по login",
            description = "Позволяет получить пользовательскую информацию по заданному логину"
    )
    public UserDto getUserByLogin(
            @Parameter(description = "Пользовательский логин, по которому будем искать информацию", example = "Greed")
            @PathVariable("login")
            String login) {
        return userService.getUserInformation(login);
    }

    @PostMapping
    @Operation(
        summary = "Сохранения пользователя",
        description = "Позволяет добавить пользователя в систему"
    )
    public UserDto createUser(@RequestBody UserDto userDto) {
      return userService.saveUserInformation(userDto);
    }

    @PutMapping("/{user-id}")
    @Operation(
            summary = "Редактирование пользовательской информации",
            description = "Позволяет изменять информацию о заданном пользователе"
    )
    public void updateUserInfo(
            @Parameter(description = "Пользовательский ID для обновления информации", example = "f7d1b2c1-cedb-4099-99d8-e4ec9302dcde")
            @PathVariable("user-id") UUID userId,
            @Parameter(description = "Сущность для обновления пользовательской информации, которая передаётся в теле запроса")
            @RequestBody UserDto userDto) {
        userService.updateUserInformation(userId, userDto);
    }
}
