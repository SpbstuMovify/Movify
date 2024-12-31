package com.polytech.contentservice.validation.user;

import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.dto.user.login.UserLoginDto;
import jakarta.validation.ConstraintValidator;
import jakarta.validation.ConstraintValidatorContext;
import lombok.RequiredArgsConstructor;
import org.apache.commons.lang3.StringUtils;
import org.springframework.stereotype.Component;

@Component
@RequiredArgsConstructor
public class ValidUserLoginParametersValidator implements
    ConstraintValidator<ValidUserLoginParameters, UserLoginDto> {
  @Override
  public void initialize(ValidUserLoginParameters constraintAnnotation) {
    ConstraintValidator.super.initialize(constraintAnnotation);
  }

  @Override
  public boolean isValid(UserLoginDto user, ConstraintValidatorContext constraintValidatorContext) {
    return (StringUtils.isNotEmpty(user.email()) && user.searchType() == UserSearchType.EMAIL)
        || (StringUtils.isNotEmpty(user.login()) && user.searchType() == UserSearchType.LOGIN);
  }
}
