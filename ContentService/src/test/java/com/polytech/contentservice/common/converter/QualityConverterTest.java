package com.polytech.contentservice.common.converter;

import static org.junit.jupiter.api.Assertions.*;

import com.polytech.contentservice.common.Quality;
import org.junit.jupiter.api.extension.ExtendWith;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.EnumSource;
import org.junit.jupiter.params.provider.NullSource;
import org.junit.jupiter.params.provider.ValueSource;
import org.mockito.Spy;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class QualityConverterTest {

  @Spy
  private QualityConverter converter;

  @ParameterizedTest
  @EnumSource(Quality.class)
  void testConvertToDatabaseColumn(Quality quality) {
    assertEquals(quality.getQuality(), converter.convertToDatabaseColumn(quality));
  }

  @ParameterizedTest
  @EnumSource(Quality.class)
  void testConvertToEntityAttribute(Quality quality) {
    assertEquals(quality, converter.convertToEntityAttribute(quality.getQuality()));
  }

  @ParameterizedTest
  @NullSource
  void testNullConvertToDatabaseColumn(Quality quality) {
    assertNull(converter.convertToDatabaseColumn(quality));
  }

  @ParameterizedTest
  @NullSource
  void testNullConvertToEntityAttribute(String quality) {
    assertNull(converter.convertToEntityAttribute(quality));
  }

  @ParameterizedTest
  @ValueSource(strings = { "999P", "P", "1P", " ", "" })
  void testConvertToEntityAttributeInvalid(String quality) {
    assertThrows(IllegalArgumentException.class, () -> converter.convertToEntityAttribute(quality));
  }
}
