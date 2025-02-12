package com.polytech.contentservice.validation.user;

import static org.junit.jupiter.api.Assertions.*;

import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import jakarta.validation.ConstraintValidatorContext;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.Spy;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class ValidUserSearchParametersValidatorTest {
  @Spy
  private ValidUserSearchParametersValidator validator;
  @Mock
  private ConstraintValidatorContext context;

  @Test
  void testValidUserIdSearch() {
    UserSearchDto user = new UserSearchDto(UUID.randomUUID(), null, null, UserSearchType.ID);
    assertTrue(validator.isValid(user, context));
  }

  @Test
  void testValidEmailSearch() {
    UserSearchDto user = new UserSearchDto(null, null, "test@example.com", UserSearchType.EMAIL);
    assertTrue(validator.isValid(user, context));
  }

  @Test
  void testValidLoginSearch() {
    UserSearchDto user = new UserSearchDto(null, "testUser", null, UserSearchType.LOGIN);
    assertTrue(validator.isValid(user, context));
  }

  @Test
  void testInvalidUserIdSearch() {
    UserSearchDto user = new UserSearchDto(null, null, null, UserSearchType.ID);
    assertFalse(validator.isValid(user, context));
  }

  @Test
  void testInvalidEmailSearch() {
    UserSearchDto user = new UserSearchDto(null, null, "", UserSearchType.EMAIL);
    assertFalse(validator.isValid(user, context));
  }

  @Test
  void testInvalidLoginSearch() {
    UserSearchDto user = new UserSearchDto(null, "", null, UserSearchType.LOGIN);
    assertFalse(validator.isValid(user, context));
  }

  @Test
  void testNullFields() {
    UserSearchDto user = new UserSearchDto(null, null, null, UserSearchType.EMAIL);
    assertFalse(validator.isValid(user, context));
  }

  @Test
  void testWrongSearchType() {
    UserSearchDto user = new UserSearchDto(UUID.randomUUID(), "testUser", "test@example.com", null);
    assertFalse(validator.isValid(user, context));
  }

}
