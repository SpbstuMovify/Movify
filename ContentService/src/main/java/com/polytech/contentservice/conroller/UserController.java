package com.polytech.contentservice.conroller;

import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.personallist.PersonalListDto;
import com.polytech.contentservice.dto.user.login.UserLoginResponseDto;
import com.polytech.contentservice.dto.user.register.UserRegistrationResponseDto;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.dto.user.login.UserLoginDto;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.mapper.UserMapper;
import com.polytech.contentservice.service.auth.AuthService;
import com.polytech.contentservice.service.personallist.PersonalListService;
import com.polytech.contentservice.service.user.UserService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import java.util.List;
import java.util.UUID;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestHeader;
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
  private final PersonalListService personalListService;
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

  @DeleteMapping("/{user-id}")
  @Operation(
      summary = "Удаление конкретного пользователя по ИД",
      description = "Позволяет удалить пользователя по заданному ИД"
  )
  public void deleteUserById(
      @Parameter(description = "Пользовательский ИД, по которому будем искать информацию", example = "f7d1b2c1-cedb-4099-99d8-e4ec9302dcde")
      @PathVariable("user-id")
      UUID userId) {
    userService.deleteById(userId);
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

  @PostMapping("/personal-list")
  @Operation(
      summary = "Сохранения понравившегося фильма или сериала",
      description = "Позволяет добавить понравившийся фильм"
  )
  public PersonalListDto addToPersonalList(@RequestBody PersonalListDto personalListDto) {
    return personalListService.addFavoriteMovie(personalListDto);
  }

  @DeleteMapping("/personal-list/{personal-list-id}")
  @Operation(
      summary = "Удалить понравившегося фильма или сериала",
      description = "Позволяет удалить понравившийся фильм"
  )
  public void deleteFromPersonalList(@PathVariable(name = "personal-list-id") UUID personalListId) {
    personalListService.removeFavoriteMovie(personalListId);
  }

  @GetMapping("/personal-list/{user-id}")
  @Operation(
      summary = "Получение понравившихся фильмов или сериала",
      description = "Позволяет получить понравившиеся фильм"
  )
  public List<ContentDto> getAllPersonalList(@PathVariable(name = "user-id") UUID userId) {
    return personalListService.getFavoriteMoviesByUser(userId);
  }

  @PostMapping("/register")
  @Operation(
      summary = "Сохранения пользователя",
      description = "Позволяет добавить пользователя в систему"
  )
  public UserRegistrationResponseDto register(@Valid @RequestBody UserRegisterDto userDto) {
    return authService.registerUser(userDto);
  }

  @PostMapping("/login")
  @Operation(
      summary = "Аунтефикация пользователя",
      description = "Позволяет войти пользователю в систему"
  )
  public UserLoginResponseDto login(@Valid @RequestBody UserLoginDto userDto,
                                    @RequestHeader String ip) {
    UserDto user = userService.getUserInformation(userMapper.toFindUserDto(userDto));
    UserDto newUser = createUserDtoWithPassword(userDto, user);
    return authService.login(newUser, ip);
  }


  @PutMapping("/role/{user-id}")
  @Operation(
      summary = "Выдача прав администратора",
      description = "Позволяет выдать пользователю права администратора"
  )
  public UserDto grantToAdmin(@PathVariable("user-id") UUID userId) {
    return userService.grantToAdmin(userId);
  }

  private UserDto createUserDtoWithPassword(UserLoginDto userDto, UserDto user) {
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
