package com.polytech.contentservice.common.converter;

import com.polytech.contentservice.common.AgeRestriction;
import jakarta.persistence.AttributeConverter;
import jakarta.persistence.Converter;

import java.util.stream.Stream;

/**
 * Конвертация возрастных ограничений из бд для entity.
 */
@Converter
public class AgeRestrictionConverter implements AttributeConverter<AgeRestriction, String> {
  @Override
  public String convertToDatabaseColumn(AgeRestriction ageRestriction) {
    if (ageRestriction == null) {
      return null;
    }

    return ageRestriction.getRestriction();
  }

  @Override
  public AgeRestriction convertToEntityAttribute(String code) {
    if (code == null) {
      return null;
    }

    return Stream.of(AgeRestriction.values())
        .filter(c -> c.getRestriction().equals(code))
        .findFirst()
        .orElseThrow(IllegalArgumentException::new);
  }
}
