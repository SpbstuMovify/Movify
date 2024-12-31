package com.polytech.contentservice.validation.user;

import jakarta.validation.Constraint;
import jakarta.validation.Payload;
import java.lang.annotation.Documented;
import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

@Documented
@Target({ElementType.TYPE})
@Retention(RetentionPolicy.RUNTIME)
@Constraint(validatedBy = ValidUserSearchParametersValidator.class)
public @interface ValidUserSearchParameters {
  String message() default "You must provide a valid email address or email address or login with specific search parameters";

  Class<?>[] groups() default {};

  Class<? extends Payload>[] payload() default {};
}
