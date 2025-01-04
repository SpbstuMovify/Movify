package com.polytech.contentservice.validation.user;

import com.polytech.contentservice.common.UserSearchType;
import com.polytech.contentservice.dto.user.search.UserSearchDto;
import jakarta.validation.ConstraintValidator;
import jakarta.validation.ConstraintValidatorContext;
import lombok.RequiredArgsConstructor;
import org.apache.commons.lang3.StringUtils;
import org.springframework.stereotype.Component;

/**
 * Валидатор для {@link UserSearchDto}.
 */
@Component
@RequiredArgsConstructor
public class ValidUserSearchParametersValidator implements
    ConstraintValidator<ValidUserSearchParameters, UserSearchDto> {
  @Override
  public boolean isValid(UserSearchDto user,
                         ConstraintValidatorContext constraintValidatorContext) {
    return (user.userId() != null && user.searchType() == UserSearchType.ID)
        || (StringUtils.isNotEmpty(user.email()) && user.searchType() == UserSearchType.EMAIL)
        || (StringUtils.isNotEmpty(user.login()) && user.searchType() == UserSearchType.LOGIN);
  }
}
