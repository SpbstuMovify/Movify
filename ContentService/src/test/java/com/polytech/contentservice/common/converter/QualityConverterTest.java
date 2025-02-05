package com.polytech.contentservice.common.converter;

import static org.junit.jupiter.api.Assertions.*;

import com.polytech.contentservice.common.Quality;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

class QualityConverterTest {

  private QualityConverter converter;

  @BeforeEach
  void setUp() {
    converter = new QualityConverter();
  }

  @Test
  void testConvertToDatabaseColumn() {
    assertEquals("720P", converter.convertToDatabaseColumn(Quality.P7200));
    assertEquals("1080P", converter.convertToDatabaseColumn(Quality.P1080));
    assertNull(converter.convertToDatabaseColumn(null));
  }

  @Test
  void testConvertToEntityAttribute() {
    assertEquals(Quality.P7200, converter.convertToEntityAttribute("720P"));
    assertEquals(Quality.P1080, converter.convertToEntityAttribute("1080P"));
    assertNull(converter.convertToEntityAttribute(null));
  }

  @Test
  void testConvertToEntityAttributeInvalid() {
    assertThrows(IllegalArgumentException.class, () -> converter.convertToEntityAttribute("999P"));
  }
}