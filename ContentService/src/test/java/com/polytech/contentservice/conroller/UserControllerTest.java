package com.polytech.contentservice.conroller;

import static org.mockito.Mockito.*;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.*;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.polytech.contentservice.common.Genre;
import com.polytech.contentservice.common.Role;
import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.personallist.PersonalListDeletionDto;
import com.polytech.contentservice.dto.personallist.PersonalListDto;
import com.polytech.contentservice.dto.user.detailed.UserDto;
import com.polytech.contentservice.dto.user.login.UserLoginDto;
import com.polytech.contentservice.dto.user.login.UserLoginResponseDto;
import com.polytech.contentservice.dto.user.register.UserRegisterDto;
import com.polytech.contentservice.dto.user.register.UserRegistrationResponseDto;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import com.polytech.contentservice.mapper.UserMapper;
import com.polytech.contentservice.service.auth.AuthService;
import com.polytech.contentservice.service.personallist.PersonalListService;
import com.polytech.contentservice.service.user.UserService;
import java.util.List;
import java.util.UUID;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.test.context.junit.jupiter.SpringExtension;
import org.springframework.test.web.servlet.MockMvc;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.MediaType;

@ExtendWith(SpringExtension.class)
@WebMvcTest(UserController.class)
class UserControllerTest {
  @Autowired
  private MockMvc mockMvc;
  @MockBean
  private UserService userService;
  @MockBean
  private UserMapper userMapper;
  @MockBean
  private PersonalListService personalListService;
  @MockBean
  private AuthService authService;
  @Autowired
  private ObjectMapper objectMapper;

  @Test
  void getUserById_ShouldReturnUser_WhenValidRequest() throws Exception {
    UserDto userDto = UserDto.builder()
        .userId(UUID.randomUUID())
        .email("john.doe@example.com")
        .build();

    doNothing().when(authService).checkTokenIsValid(any(), any());
    when(userService.getUserById(any())).thenReturn(userDto);

    String json = mockMvc.perform(get("/v1/users/{userId}", userDto.userId())
            .header("Authorization", "Bearer valid-token")
            .contentType(MediaType.APPLICATION_JSON))
        .andExpect(status().isOk())
        .andReturn()
        .getResponse().getContentAsString();
    UserDto user = objectMapper.readValue(json, UserDto.class);
    Assertions.assertNotNull(user);
    Assertions.assertEquals(userDto.email(), user.email());
  }

  @Test
  void deleteUserById_thenSuccess() throws Exception {
    UUID userId = UUID.randomUUID();
    String token = "Bearer valid-token";

    doNothing().when(authService).checkTokenIsValid(any(), any());
    doNothing().when(userService).deleteById(userId);

    mockMvc.perform(delete("/v1/users/{user_id}", userId)
            .header("Authorization", token))
        .andExpect(status().isOk());
  }

  @Test
  void login_thenSuccess() throws Exception {
    UserLoginDto loginUserDto = UserLoginDto.builder()
        .password("password")
        .searchType(UserSearchType.LOGIN)
        .email("john.doe@example.com")
        .login("aaaa")
        .build();
    UserDto userDto = UserDto.builder()
        .userId(UUID.randomUUID())
        .email("john.doe@example.com")
        .login("aaaa")
        .build();
    String token = "Bearer valid-token";
    UserLoginResponseDto loginResponseDto = UserLoginResponseDto.builder()
        .login(userDto.login())
        .email(userDto.email())
        .token("aaa")
        .role(Role.USER)
        .build();
    when(userService.getUserInformation(any())).thenReturn(userDto);
    when(authService.login(any(), any())).thenReturn(loginResponseDto);

    String result = mockMvc.perform(post("/v1/users/login")
            .content(objectMapper.writeValueAsString(loginUserDto))
            .contentType(MediaType.APPLICATION_JSON)
            .header("Authorization", token)
            .header("ip", "111.111.111.111"))
        .andExpect(status().isOk())
        .andReturn()
        .getResponse()
        .getContentAsString();
    UserLoginResponseDto user = objectMapper.readValue(result, UserLoginResponseDto.class);
    Assertions.assertNotNull(user);
    Assertions.assertEquals(loginResponseDto.email(), user.email());
    Assertions.assertEquals(loginResponseDto.login(), user.login());
    Assertions.assertEquals(loginResponseDto.role(), user.role());
    Assertions.assertEquals(loginResponseDto.token(), user.token());
  }

  @Test
  void resetPassword_thenSuccess() throws Exception {
    UserRegisterDto userDto = UserRegisterDto.builder()
        .firstName("John")
        .email("john.doe@example.com")
        .lastName("Doe")
        .login("john_appp")
        .password("password")
        .role(Role.USER)
        .build();
    String token = "Bearer valid-token";

    UserRegistrationResponseDto response = UserRegistrationResponseDto.builder()
        .userId(UUID.randomUUID())
        .login(userDto.login())
        .email(userDto.email())
        .build();

    doNothing().when(authService).checkTokenIsValid(any(), any());
    when(authService.resetUserPassword(any())).thenReturn(response);

    String json = mockMvc.perform(post("/v1/users/password-recovery")
            .header("Authorization", token)
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(userDto)))
        .andExpect(status().isOk())
        .andReturn()
        .getResponse()
        .getContentAsString();

    UserDto user = objectMapper.readValue(json, UserDto.class);
    Assertions.assertNotNull(user);
    Assertions.assertEquals(response.email(), user.email());
    Assertions.assertEquals(response.token(), user.token());
  }

  @Test
  void register_ShouldReturnUserRegistrationResponse() throws Exception {
    UserRegisterDto userDto = UserRegisterDto.builder()
        .firstName("John")
        .email("john.doe@example.com")
        .lastName("Doe")
        .login("john_appp")
        .password("password")
        .role(Role.USER)
        .build();

    UserRegistrationResponseDto responseDto = UserRegistrationResponseDto.builder()
        .userId(UUID.randomUUID())
        .email(userDto.email())
        .login(userDto.login())
        .token("aaaa")
        .build();

    when(authService.registerUser(any())).thenReturn(responseDto);

    mockMvc.perform(post("/v1/users/register")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(userDto)))
        .andExpect(status().isOk())
        .andExpect(jsonPath("$.token").value(responseDto.token()));
  }

  @Test
  void findUser() throws Exception {
    UserSearchDto userDto = UserSearchDto.builder()
        .userId(UUID.randomUUID())
        .searchType(UserSearchType.ID)
        .build();

    UserDto response = UserDto.builder()
        .userId(userDto.userId())
        .build();

    doNothing().when(authService).checkTokenIsValid(any(), any());
    when(userService.getUserInformation(any())).thenReturn(response);

    String json = mockMvc.perform(post("/v1/users/info")
            .header("Authorization", "Bearer valid-token")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(userDto)))
        .andExpect(status().isOk())
        .andReturn()
        .getResponse()
        .getContentAsString();

    UserDto user = objectMapper.readValue(json, UserDto.class);
    Assertions.assertNotNull(user);
    Assertions.assertEquals(response.userId(), user.userId());
  }

  @Test
  void addToPersonalList() throws Exception {
    PersonalListDto personalListDto = PersonalListDto.builder()
        .userId(UUID.randomUUID())
        .contentId(UUID.randomUUID())
        .build();

    doNothing().when(authService).checkTokenIsValid(any(), any());
    when(personalListService.addFavoriteMovie(any())).thenReturn(personalListDto);

    String json = mockMvc.perform(post("/v1/users/personal-list")
            .header("Authorization", "Bearer valid-token")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(personalListDto)))
        .andExpect(status().isOk())
        .andReturn()
        .getResponse()
        .getContentAsString();

    PersonalListDto personalList = objectMapper.readValue(json, PersonalListDto.class);
    Assertions.assertNotNull(personalList);
    Assertions.assertEquals(personalListDto.userId(), personalList.userId());
    Assertions.assertEquals(personalListDto.contentId(), personalList.contentId());
  }

  @Test
  void deleteFromPersonalList() throws Exception {
    PersonalListDeletionDto personalListDeletionDto = PersonalListDeletionDto.builder()
        .userId(UUID.randomUUID())
        .contentId(UUID.randomUUID())
        .build();

    doNothing().when(authService).checkTokenIsValid(any(), any());
    doNothing().when(personalListService).removeFavoriteMovie(any());

    mockMvc.perform(delete("/v1/users/personal-list")
            .header("Authorization", "Bearer valid-token")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(personalListDeletionDto)))
        .andExpect(status().isOk());

    verify(authService).checkTokenIsValid(any(), any());
    verify(personalListService).removeFavoriteMovie(any());
  }

  @Test
  void getAllPersonalList() throws Exception {
    List<ContentDto> response = List.of(ContentDto.builder().title("SSS").genre(Genre.BLOCKBUSTER).year(2020).build());

    doNothing().when(authService).checkTokenIsValid(any(), any());
    when(personalListService.getFavoriteMoviesByUser(any())).thenReturn(response);

    mockMvc.perform(get("/v1/users/personal-list/{user-id}", UUID.randomUUID())
            .header("Authorization", "Bearer valid-token")
            .contentType(MediaType.APPLICATION_JSON))
        .andExpect(status().isOk());

    verify(authService).checkTokenIsValid(any(), any());
    verify(personalListService).getFavoriteMoviesByUser(any());
  }

  @Test
  void grantToAdmin() throws Exception {
    UserDto userDto = UserDto.builder()
        .userId(UUID.randomUUID())
        .searchType(UserSearchType.ID)
        .build();

    doNothing().when(authService).checkTokenIsValid(any(), any());
    when(userService.grantToAdmin(any())).thenReturn(userDto);

    String json = mockMvc.perform(put("/v1/users/role/{user-id}", userDto.userId())
            .header("Authorization", "Bearer valid-token")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(userDto)))
        .andExpect(status().isOk())
        .andReturn()
        .getResponse()
        .getContentAsString();

    UserDto user = objectMapper.readValue(json, UserDto.class);
    Assertions.assertNotNull(user);
    Assertions.assertEquals(userDto.userId(), user.userId());
  }

  @Test
  void updateUserInfo() throws Exception {
    UserDto userDto = UserDto.builder()
        .userId(UUID.randomUUID())
        .searchType(UserSearchType.ID)
        .email("john.doe@example.com")
        .firstName("John")
        .build();

    doNothing().when(authService).checkTokenIsValid(any(), any());
    doNothing().when(userService).updateUserInformation(any(), any());

    mockMvc.perform(put("/v1/users/{user-id}", userDto.userId())
            .header("Authorization", "Bearer valid-token")
            .contentType(MediaType.APPLICATION_JSON)
            .content(objectMapper.writeValueAsString(userDto)))
        .andExpect(status().isOk());

    verify(authService).checkTokenIsValid(any(), any());
    verify(userService).updateUserInformation(any(), any());
  }
}
