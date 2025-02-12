package com.polytech.contentservice.validation.user;

import static org.junit.jupiter.api.Assertions.*;

import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.dto.user.login.UserLoginDto;
import jakarta.validation.ConstraintValidatorContext;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.Spy;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class ValidUserLoginParametersValidatorTest {
  @Spy
  private ValidUserLoginParametersValidator validator;
  @Mock
  private ConstraintValidatorContext context;

  @Test
  void testValidEmailLogin() {
    UserLoginDto user = new UserLoginDto(null, "test@example.com", "password", UserSearchType.EMAIL);
    assertTrue(validator.isValid(user, context));
  }

  @Test
  void testValidUsernameLogin() {
    UserLoginDto user = new UserLoginDto("testUser", null, "password", UserSearchType.LOGIN);
    assertTrue(validator.isValid(user, context));
  }

  @Test
  void testInvalidEmailLogin() {
    UserLoginDto user = new UserLoginDto(null, "", "password", UserSearchType.EMAIL);
    assertFalse(validator.isValid(user, context));
  }

  @Test
  void testInvalidUsernameLogin() {
    UserLoginDto user = new UserLoginDto("", null, "password", UserSearchType.LOGIN);
    assertFalse(validator.isValid(user, context));
  }

  @Test
  void testNullFields() {
    UserLoginDto user = new UserLoginDto(null, null, "password", UserSearchType.EMAIL);
    assertFalse(validator.isValid(user, context));
  }

  @Test
  void testWrongSearchType() {
    UserLoginDto user = new UserLoginDto("testUser", "test@example.com", "password", null);
    assertFalse(validator.isValid(user, context));
  }
}
