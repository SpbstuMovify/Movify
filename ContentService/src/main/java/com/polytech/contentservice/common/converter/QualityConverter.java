package com.polytech.contentservice.common.converter;

import com.polytech.contentservice.common.Quality;
import jakarta.persistence.AttributeConverter;
import jakarta.persistence.Converter;
import java.util.stream.Stream;

/**
 * Конвертация жанров из бд для entity.
 */
@Converter
public class QualityConverter implements AttributeConverter<Quality, String> {
  @Override
  public String convertToDatabaseColumn(Quality quality) {
    if (quality == null) {
      return null;
    }

    return quality.getQuality();
  }

  @Override
  public Quality convertToEntityAttribute(String code) {
    if (code == null) {
      return null;
    }

    return Stream.of(Quality.values())
        .filter(c -> c.getQuality().equals(code))
        .findFirst()
        .orElseThrow(IllegalArgumentException::new);
  }
}
