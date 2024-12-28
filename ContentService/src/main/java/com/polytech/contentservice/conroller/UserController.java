package com.polytech.contentservice.conroller;

import com.polytech.contentservice.dto.user.login.UserLoginResponseDto;
import com.polytech.contentservice.dto.user.register.UserRegistrationResponseDto;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.dto.user.login.UserLoginDto;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.mapper.UserMapper;
import com.polytech.contentservice.service.auth.AuthService;
import com.polytech.contentservice.service.user.UserService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
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
    private final AuthService authService;
    private final UserMapper userMapper;

    @GetMapping("/{user-id}")
    @Operation(
            summary = "Получения конкретного пользователя по ИД",
            description = "Позволяет получить пользовательскую информацию по заданному ИД"
    )
    public UserDto getUserById(
            @Parameter(description = "Пользовательский ИД, по которому будем искать информацию", example = "f7d1b2c1-cedb-4099-99d8-e4ec9302dcde")
            @PathVariable("user-id")
            UUID userId) {
        return userService.getUserById(userId);
    }

    @PostMapping("/info")
    @Operation(
            summary = "Получения конкретного пользователя по некоторой информации",
            description = "Позволяет получить пользовательскую информацию по некоторой информации: email/login/id"
    )
    public UserDto findUser(
            @Parameter(description = "Пользовательские данные, по которым будем искать информацию")
            @Valid @RequestBody UserSearchDto userDto) {
        return userService.getUserInformation(userDto);
    }

    @PostMapping("/register")
    @Operation(
        summary = "Сохранения пользователя",
        description = "Позволяет добавить пользователя в систему"
    )
    public UserRegistrationResponseDto register(@Valid @RequestBody UserRegisterDto userDto) {
      UserDto savedUser = userService.saveUserInformation(userDto);
      return authService.registerUser(savedUser);
    }

  @PostMapping("/login")
  @Operation(
      summary = "Аунтефикация пользователя",
      description = "Позволяет войти пользователю в систему"
  )
  public UserLoginResponseDto login(@Valid @RequestBody UserLoginDto userDto) {
    UserDto user = userService.getUserInformation(userMapper.toFindUserDto(userDto));
    return authService.login(user);
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
